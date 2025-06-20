using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Data;
using static ToSic.Eav.Apps.Sys.AppStack.AppStackConstants;
using SettingsSources = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, ToSic.Eav.Data.Sys.IPropertyLookup>>;

namespace ToSic.Sxc.Code.Internal;

public partial class ExecutionContext
{
    /// <inheritdoc />
    [PublicApi]
    public IDynamicStack Resources => _resources.Get(() => Cdf.AsDynStack(RootNameResources, ResSrc))!;
    private readonly GetOnce<IDynamicStack> _resources = new();

    [PrivateApi]
    public ITypedStack AllResources => _allRes.Get(() => Cdf.AsTypedStack(RootNameResources, ResSrc))!;
    private readonly GetOnce<ITypedStack> _allRes= new();

    [field: AllowNull, MaybeNull]
    private AppDataStackService AppDss => field ??= Services.DataStackService.Init(((IAppWithInternal)App).AppReader);

    private SettingsSources ResSrc => _resSrc.Get(() => AppDss.GetStack(AppStackConstants.Resources, Block?.View?.Resources))!;
    private readonly GetOnce<SettingsSources> _resSrc = new();


    private SettingsSources SetSrc => _setSrc.Get(() => AppDss.GetStack(AppStackConstants.Settings, Block?.View?.Settings))!;
    private readonly GetOnce<SettingsSources> _setSrc = new();

    /// <inheritdoc />
    [PublicApi]
    public IDynamicStack Settings => _settings.Get(() => Cdf.AsDynStack(RootNameSettings, SetSrc))!;
    private readonly GetOnce<IDynamicStack> _settings = new();

    public ITypedStack AllSettings => _allSettings.Get(() => Cdf.AsTypedStack(RootNameSettings, SetSrc))!;
    private readonly GetOnce<ITypedStack> _allSettings = new();

    // TODO: 2025-05-11 2dm WIP
    //dynamic IDynamicCode12.Resources => Resources;
    //dynamic IDynamicCode12.Settings => Settings;
}