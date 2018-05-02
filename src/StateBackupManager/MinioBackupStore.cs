
namespace ServiceFabricContrib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Fabric.Description;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Minio;
    using System.Reactive.Linq;

    public class MinioBackupManager : IBackupStore
    {
        private readonly MinioClient minio;
        private readonly string bucketName;
        private int MaxBackupsToKeep;

        private string PartitionTempDirectory;
        private string partitionId;

        private long backupFrequencyInSeconds;
        private long keyMin;
        private long keyMax;

        public MinioBackupManager(ConfigurationSection configSection, string partitionId, long keymin, long keymax, string codePackageTempDirectory)
        {
            this.keyMin = keymin;
            this.keyMax = keymax;

            var endpoint = configSection.Parameters["Endpoint"].Value;
            var accessKey = configSection.Parameters["AccessKey"].Value;
            var secretKey = configSection.Parameters["SecretKey"].Value;
            var withSSL = configSection.Parameters.Contains("WithSSL")
                ? bool.Parse(configSection.Parameters["WithSSL"].Value)
                : false;

            minio = withSSL
                ? new MinioClient(endpoint, accessKey, secretKey).WithSSL()
                : new MinioClient(endpoint, accessKey, secretKey);

            this.backupFrequencyInSeconds = configSection.Parameters.Contains("BackupFrequencyInSeconds")
                ? long.Parse(configSection.Parameters["BackupFrequencyInSeconds"].Value)
                : 60;
            this.MaxBackupsToKeep = configSection.Parameters.Contains("MaxBackupsToKeep")
                ? int.Parse(configSection.Parameters["MaxBackupsToKeep"].Value)
                : 10;
            this.partitionId = partitionId;
            this.PartitionTempDirectory = Path.Combine(codePackageTempDirectory, partitionId);
            this.bucketName = "sfbackup" + partitionId;

        }

        public long BackupFrequencyInSeconds
        {
            get { return this.backupFrequencyInSeconds; }
        }

        private async Task EnsureBucket()
        {
            bool found = await minio.BucketExistsAsync(bucketName);
            if (!found)
            {
                await minio.MakeBucketAsync(bucketName);
            }
        }

        public async Task ArchiveBackupAsync(BackupInfo backupInfo, CancellationToken cancellationToken)
        {
            Trace.TraceInformation("MinioBackupManager: Archive Called.");

            string fullArchiveDirectory = Path.Combine(this.PartitionTempDirectory, Guid.NewGuid().ToString("N"));

            DirectoryInfo fullArchiveDirectoryInfo = new DirectoryInfo(fullArchiveDirectory);
            fullArchiveDirectoryInfo.Create();

            string blobName = string.Format("{0}_{1}_{2}_{3}", Guid.NewGuid().ToString("N"), this.keyMin, this.keyMax, "Backup.zip");
            string fullArchivePath = Path.Combine(fullArchiveDirectory, "Backup.zip");

            ZipFile.CreateFromDirectory(backupInfo.Directory, fullArchivePath, CompressionLevel.Fastest, false);

            DirectoryInfo backupDirectory = new DirectoryInfo(backupInfo.Directory);
            backupDirectory.Delete(true);

            await EnsureBucket();
            await minio.PutObjectAsync(bucketName, blobName, fullArchivePath);

            DirectoryInfo tempDirectory = new DirectoryInfo(fullArchiveDirectory);
            tempDirectory.Delete(true);

            Trace.TraceInformation("MinioBackupManager: UploadBackupFolderAsync: success.");
        }

        public async Task<string> RestoreLatestBackupToTempLocation(CancellationToken cancellationToken)
        {
            Trace.TraceInformation("MinioBackupManager: Download backup async called.");

            var lastBackupBlob = (await this.GetBackupBlobs(true)).First();

            Trace.TraceInformation("MinioBackupManager: Downloading {0}", lastBackupBlob.Key);

            string downloadId = Guid.NewGuid().ToString("N");

            string zipPath = Path.Combine(this.PartitionTempDirectory, string.Format("{0}_Backup.zip", downloadId));

            await EnsureBucket();
            await minio.GetObjectAsync(bucketName, lastBackupBlob.Key, zipPath);

            string restorePath = Path.Combine(this.PartitionTempDirectory, downloadId);

            ZipFile.ExtractToDirectory(zipPath, restorePath);

            FileInfo zipInfo = new FileInfo(zipPath);
            zipInfo.Delete();

            Trace.TraceInformation("MinioBackupManager: Downloaded {0} in to {1}", lastBackupBlob.Key, restorePath);

            return restorePath;
        }

        public async Task DeleteBackupsAsync(CancellationToken cancellationToken)
        {
            if (await minio.BucketExistsAsync(bucketName))
            {
                Trace.TraceInformation("MinioBackupManager: Deleting old backups");

                var oldBackups = (await this.GetBackupBlobs(true)).Skip(this.MaxBackupsToKeep);

                foreach (var backup in oldBackups)
                {
                    Trace.TraceInformation("MinioBackupManager: Deleting {0}", backup.Key);
                    await minio.RemoveObjectAsync(bucketName, backup.Key);
                }
            }
        }

        private async Task<IEnumerable<Minio.DataModel.Item>> GetBackupBlobs(bool sorted)
        {
            var observable = minio.ListObjectsAsync(bucketName);

            var itemizedBlobs = new List<Minio.DataModel.Item>();
            await observable.ForEachAsync(item => itemizedBlobs.Add(item));

            Trace.TraceInformation("MinioBackupManager: Got {0} blobs", itemizedBlobs.Count);

            if (sorted)
            {
                return itemizedBlobs.OrderByDescending(x => x.LastModified);
            }
            else
            {
                return itemizedBlobs;
            }
        }
    }
}
