using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceFabricContrib
{
    public enum BackupManagerType
    {
        Azure,
        Minio,
        Local,
        None
    }
}
