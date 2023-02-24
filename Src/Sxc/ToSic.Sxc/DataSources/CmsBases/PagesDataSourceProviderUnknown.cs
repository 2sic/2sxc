using System.Collections.Generic;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.DataSources
{
    public class PagesDataSourceProviderUnknown: PagesDataSourceProvider
    {
        public PagesDataSourceProviderUnknown(WarnUseOfUnknown<PagesDataSourceProviderUnknown> _): base($"{Constants.SxcLogName}.{LogConstants.NameUnknown}")
        { }

        public override List<PageDataNew> GetPagesInternal(
            string noParamOrder = Eav.Parameters.Protector,
            bool includeHidden = default,
            bool includeDeleted = default,
            bool includeAdmin = default,
            bool includeSystem = default,
            bool includeLinks = default,
            bool requireViewPermissions = true,
            bool requireEditPermissions = true
        ) => new List<PageDataNew>();
    }
}
