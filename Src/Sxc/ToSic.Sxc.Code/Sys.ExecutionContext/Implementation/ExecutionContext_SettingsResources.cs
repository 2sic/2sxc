using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Data;
using static ToSic.Eav.Apps.Sys.AppStack.AppStackConstants;
using SettingsSources = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, ToSic.Eav.Data.Sys.PropertyLookup.IPropertyLookup>>;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext
{
    /// <inheritdoc />
    [PublicApi]
    public IDynamicStack Resources => field ??= Cdf.AsDynStack(RootNameResources, ResSrc);

    [PrivateApi]
    public ITypedStack AllResources => field ??= Cdf.AsTypedStack(RootNameResources, ResSrc);

    [field: AllowNull, MaybeNull]
    private AppDataStackService AppDss => field ??= Services.DataStackService.Init(((IAppWithInternal)App).AppReader);

    private SettingsSources ResSrc => field ??= AppDss.GetStack(AppStackConstants.Resources, Block?.View?.Resources);


    private SettingsSources SetSrc => field ??= AppDss.GetStack(AppStackConstants.Settings, Block?.View?.Settings);

    /// <inheritdoc />
    [PublicApi]
    public IDynamicStack Settings => field ??= Cdf.AsDynStack(RootNameSettings, SetSrc);

    public ITypedStack AllSettings => field ??= Cdf.AsTypedStack(RootNameSettings, SetSrc);

}