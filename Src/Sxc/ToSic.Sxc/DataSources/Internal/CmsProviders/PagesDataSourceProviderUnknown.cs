using ToSic.Eav.Internal.Unknown;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.DataSources.Internal;

internal class PagesDataSourceProviderUnknown(WarnUseOfUnknown<PagesDataSourceProviderUnknown> _) : PagesDataSourceProvider($"{SxcLogName}.{LogConstants.NameUnknown}")
{
    public override List<PageDataRaw> GetPagesInternal(NoParamOrder noParamOrder = default,
        bool includeHidden = default, bool includeDeleted = default, bool includeAdmin = default,
        bool includeSystem = default, bool includeLinks = default, bool requireViewPermissions = true,
        bool requireEditPermissions = true) => [];
}