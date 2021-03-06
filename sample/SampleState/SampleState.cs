﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Description;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContribSample.Contracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabricContrib;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using MassTransit;

namespace SampleState
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class SampleState : StatefulService, ISampleRemotingService
    {
        public SampleState(StatefulServiceContext context)
            : base(context)
        { }

        public Task<List<PeopleDto>> GetPeoplesAsync(FilterDto input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return Task.FromResult(new List<PeopleDto>
            {
                new PeopleDto{Name ="abc",Age =1,Toys=new List<ToyDto>{new ToyDto{Name="Lego",Amount=100} } },
                new PeopleDto{Name="efg",Age=2,Toys=new List<ToyDto>{new ToyDto{Name="Lego",Amount=50} }}
            });
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            //use bson serialization
            return new[]
            {
                //MassTransitListener can not be used in stateful service 
                //new ServiceReplicaListener(_ => new MassTransitListener(bus), "masstransit"),
                new ServiceReplicaListener((c) =>new FabricTransportServiceRemotingListener(c, this,
                    serializationProvider:new BsonSerializationProvider()))
            };
        }

        //private IBusControl _bus;
        //protected override async Task OnOpenAsync(ReplicaOpenMode openMode, CancellationToken cancellationToken)
        //{
        //    await base.OnOpenAsync(openMode, cancellationToken);

        //    ServiceEventSource.Current.ServiceMessage(Context, "Starting Masstrasit...");
        //    //ref: https://stackoverflow.com/a/41371221
        //    _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        //    {
        //        var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
        //        {
        //            h.Username("guest");
        //            h.Password("guest");
        //        });

        //        cfg.ReceiveEndpoint(host, "ServiceFabricContrib.SampleState", c =>
        //        {
        //            c.Handler<FilterDto>(context =>
        //            {
        //                ServiceEventSource.Current.ServiceMessage(Context, $"Received: {context.Message.Search}");
        //                return Task.CompletedTask;
        //            });
        //        });
        //    });
        //    await _bus.StartAsync();
        //    ServiceEventSource.Current.ServiceMessage(Context, "Started Masstrasit...");
        //}

        //protected override async Task OnCloseAsync(CancellationToken cancellationToken)
        //{
        //    ServiceEventSource.Current.ServiceMessage(Context, "Stoping Masstrasit...");
        //    await _bus?.StopAsync();
        //    ServiceEventSource.Current.ServiceMessage(Context, "Stoped Masstrasit...");

        //    await base.OnCloseAsync(cancellationToken);
        //}

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                ServiceEventSource.Current.ServiceMessage(Context, "inside RunAsync for SampleState Service");

                //return Task.WhenAll(
                //    this.PeriodicCounter(cancellationToken),
                //    this.PeriodicTakeBackupAsync(cancellationToken));

                //ref: https://stackoverflow.com/a/41371221
                //NOTE use Run instead of Open for primary replica
                var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint(host, "ServiceFabricContrib.SampleState", c =>
                    {
                        c.Handler<FilterDto>(context =>
                        {
                            ServiceEventSource.Current.ServiceMessage(Context, $"Received: {context.Message.Search}");
                            return Task.CompletedTask;
                        });
                    });
                });
                await bus.StartAsync();

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        //Service Fabric wants us to stop
                        await bus.StopAsync();

                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch (OperationCanceledException e)
            {
                ServiceEventSource.Current.ServiceMessage(Context, "RunAsync Stoped, {0}", e);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(Context, "RunAsync Failed, {0}", e);
                throw;
            }
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        private async Task PeriodicCounter(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
            }
        }

        private IBackupStore backupManager;

        private BackupManagerType backupStorageType;

        private void SetupBackupManager()
        {
            string partitionId = this.Context.PartitionId.ToString("N");
            Debug.WriteLine("partitionId: " + partitionId);
            long minKey = ((Int64RangePartitionInformation)this.Partition.PartitionInfo).LowKey;
            long maxKey = ((Int64RangePartitionInformation)this.Partition.PartitionInfo).HighKey;

            if (this.Context.CodePackageActivationContext != null)
            {
                ICodePackageActivationContext codePackageContext = this.Context.CodePackageActivationContext;
                ConfigurationPackage configPackage = codePackageContext.GetConfigurationPackageObject("Config");

                this.backupStorageType = BackupManagerType.Minio;

                ConfigurationSection minioBackupConfigSection = configPackage.Settings.Sections["BackupSettings.Minio"];

                this.backupManager = new MinioBackupManager(minioBackupConfigSection, partitionId, minKey, maxKey, codePackageContext.TempDirectory);


                ServiceEventSource.Current.ServiceMessage(Context, "Backup Manager Set Up");
            }
        }

        private async Task PeriodicTakeBackupAsync(CancellationToken cancellationToken)
        {
            long backupsTaken = 0;
            this.SetupBackupManager();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (this.backupStorageType == BackupManagerType.None)
                {
                    break;
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(this.backupManager.BackupFrequencyInSeconds));


                    BackupDescription backupDescription = new BackupDescription(BackupOption.Full, this.BackupCallbackAsync);

                    await this.BackupAsync(backupDescription, TimeSpan.FromHours(1), cancellationToken);

                    backupsTaken++;

                    ServiceEventSource.Current.ServiceMessage(Context, "Backup {0} taken", backupsTaken);
                }
            }
        }

        private const string BackupCountDictionaryName = "BackupCountingDictionary";

        private async Task<bool> BackupCallbackAsync(BackupInfo backupInfo, CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(Context, "Inside backup callback for replica {0}|{1}", this.Context.PartitionId, this.Context.ReplicaId);
            long totalBackupCount;

            IReliableDictionary<string, long> backupCountDictionary =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>(BackupCountDictionaryName);
            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                ConditionalValue<long> value = await backupCountDictionary.TryGetValueAsync(tx, "backupCount");

                if (!value.HasValue)
                {
                    totalBackupCount = 0;
                }
                else
                {
                    totalBackupCount = value.Value;
                }

                await backupCountDictionary.SetAsync(tx, "backupCount", ++totalBackupCount);

                await tx.CommitAsync();
            }

            ServiceEventSource.Current.Message("Backup count dictionary updated, total backup count is {0}", totalBackupCount);

            try
            {
                ServiceEventSource.Current.ServiceMessage(Context, "Archiving backup");
                await this.backupManager.ArchiveBackupAsync(backupInfo, cancellationToken);
                ServiceEventSource.Current.ServiceMessage(Context, "Backup archived");
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(Context, "Archive of backup failed: Source: {0} Exception: {1}", backupInfo.Directory, e.Message);
            }

            try
            {
                ServiceEventSource.Current.ServiceMessage(Context, "Deleting backups");
                await this.backupManager.DeleteBackupsAsync(cancellationToken);
                ServiceEventSource.Current.ServiceMessage(Context, "Backups deleted");
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(Context, "Delete of backup failed: Exception: {1}", e.Message);
            }

            return true;
        }

        //protected override async Task<bool> OnDataLossAsync(RestoreContext restoreCtx, CancellationToken cancellationToken)
        //{
        //    ServiceEventSource.Current.ServiceMessage(Context, "OnDataLoss Invoked!");
        //    this.SetupBackupManager();

        //    try
        //    {
        //        string backupFolder;

        //        if (this.backupStorageType == BackupManagerType.None)
        //        {
        //            //since we have no backup configured, we return false to indicate
        //            //that state has not changed. This replica will become the basis
        //            //for future replica builds
        //            return false;
        //        }
        //        else
        //        {
        //            backupFolder = await this.backupManager.RestoreLatestBackupToTempLocation(cancellationToken);
        //        }

        //        ServiceEventSource.Current.ServiceMessage(Context, "Restoration Folder Path " + backupFolder);

        //        RestoreDescription restoreRescription = new RestoreDescription(backupFolder, RestorePolicy.Force);

        //        await restoreCtx.RestoreAsync(restoreRescription, cancellationToken);

        //        ServiceEventSource.Current.ServiceMessage(Context, "Restore completed");

        //        DirectoryInfo tempRestoreDirectory = new DirectoryInfo(backupFolder);
        //        tempRestoreDirectory.Delete(true);

        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        ServiceEventSource.Current.ServiceMessage(Context, "Restoration failed: " + "{0} {1}" + e.GetType() + e.Message);

        //        throw;
        //    }
        //}
    }
}
