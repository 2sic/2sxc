using System.Collections.Generic;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Coding;
using ToSic.Lib.Logging;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.DataSources;

internal class PagesDataSourceProviderUnknown: PagesDataSourceProvider
{
    public PagesDataSourceProviderUnknown(WarnUseOfUnknown<PagesDataSourceProviderUnknown> _): base($"{SxcLogging.SxcLogName}.{LogConstants.NameUnknown}")
    { }

    public override List<PageDataRaw> GetPagesInternal(NoParamOrder noParamOrder = default,
        bool includeHidden = default, bool includeDeleted = default, bool includeAdmin = default,
        bool includeSystem = default, bool includeLinks = default, bool requireViewPermissions = true,
        bool requireEditPermissions = true) => new();
}