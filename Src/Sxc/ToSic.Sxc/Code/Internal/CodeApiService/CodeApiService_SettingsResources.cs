using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Services;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Stack;
using static ToSic.Eav.Apps.AppStackConstants;
using SettingsSources = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, ToSic.Eav.Data.PropertyLookup.IPropertyLookup>>;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService
{
    /// <inheritdoc />
    [PublicApi]
    public IDynamicStack Resources => _resources.Get(() => Cdf.AsDynStack(RootNameResources, ResSrc));
    private readonly GetOnce<DynamicStack> _resources = new();

    [PrivateApi]
    public ITypedStack AllResources => _allRes.Get(() => Cdf.AsTypedStack(RootNameResources, ResSrc));
    private readonly GetOnce<ITypedStack> _allRes= new();

    private AppDataStackService AppSS => _appSetStackService ??= Services.DataStackService.Init(((IAppWithInternal)App).AppReader);
    private AppDataStackService _appSetStackService;

    private SettingsSources ResSrc => _resSrc.Get(() => AppSS.GetStack(AppStackConstants.Resources, _Block?.View?.Resources));
    private readonly GetOnce<SettingsSources> _resSrc = new();


    private SettingsSources SetSrc => _setSrc.Get(() => AppSS.GetStack(AppStackConstants.Settings, _Block?.View?.Settings));
    private readonly GetOnce<SettingsSources> _setSrc = new();

    /// <inheritdoc />
    [PublicApi]
    public IDynamicStack Settings => _settings.Get(() => Cdf.AsDynStack(RootNameSettings, SetSrc));
    private readonly GetOnce<DynamicStack> _settings = new();

    public ITypedStack AllSettings => _allSettings.Get(() => Cdf.AsTypedStack(RootNameSettings, SetSrc));
    private readonly GetOnce<ITypedStack> _allSettings = new();

    dynamic IDynamicCode12.Resources => Resources;
    dynamic IDynamicCode12.Settings => Settings;
}