using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public class AdamDataSourceProviderUnknown<TFolderId, TFileId> : AdamDataSourceProvider<TFolderId, TFileId>
    {
        public AdamDataSourceProviderUnknown(MyServices services, WarnUseOfUnknown<AppFilesDataSourceProviderUnknown> _) : base(services, $"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        {
        }

        public new AdamDataSourceProvider<TFolderId, TFileId> Configure(
            string noParamOrder = Eav.Parameters.Protector,
            int appId = default,
            string entityIds = default,
            string entityGuids = default,
            string fields = default,
            string filter = default
        ) => this;

        public new Func<IEntity, IEnumerable<AdamItemDataRaw>> GetInternal() => (entity) => new List<AdamItemDataRaw>();
    }
}