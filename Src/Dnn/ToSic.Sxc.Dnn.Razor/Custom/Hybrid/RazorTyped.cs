using Custom.Razor.Sys;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Custom.Hybrid;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.Razor;
using ToSic.Sxc.Dnn.Razor.Sys;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.Help;
using static System.StringComparer;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Base class for v16 [Typed](xref:NetCode.TypedCode.Index) Razor files.
/// Use it to create custom CS code in your App.
/// 
/// It provides the <see cref="ServiceKit16"/> on property `Kit` which contains all the popular services to create amazing stuff.
/// </summary>
/// <remarks>
/// Important: This is very different from Razor12 or Razor14, as it doesn't rely on `dynamic` code.
/// Be aware of this since the APIs are very different - see [Typed Code](xref:NetCode.TypedCode.Index).
/// </remarks>
[PublicApi]
public abstract class RazorTyped: RazorComponentBase, IRazor, ITypedCode16, IHasCodeHelp, IGetCodePath, ISetDynamicModel, ICanUseRoslynCompiler
{
    #region Constructor, Setup, Helpers

    internal ICodeTypedApiHelper CodeApi => field
        ??= ExCtx.GetTypedApi();


    /// <inheritdoc cref="DnnRazorHelper.RenderPageNotSupported"/>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public override HelperResult RenderPage(string path, params object[] data)
        => RzrHlp.RenderPageNotSupported();


    /// <inheritdoc cref="ICompatibilityLevel.CompatibilityLevel"/>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel16;

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    /// <inheritdoc cref="ITypedCode16.GetService{TService}(NoParamOrder, string?)"/>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TService GetService<TService>(NoParamOrder protector = default, string typeName = default) where TService : class
        => AppCodeGetNamedServiceHelper.GetService<TService>(owner: this, CodeHelper.Specs, typeName);

    /// <inheritdoc cref="IHasKit{TServiceKit}.Kit"/>
    public ServiceKit16 Kit => field ??= CodeApi.ServiceKit16;

    internal TypedCode16Helper CodeHelper => field ??= CreateCodeHelper();

    /// <inheritdoc cref="CodeTyped.Customize"/>
    protected ICodeCustomizer Customize => field ??= CodeApi.GetService<ICodeCustomizer>(reuse: true);

    void ISetDynamicModel.SetDynamicModel(RenderSpecs viewData)
    {
        _renderSpecs = viewData;

        // Only overwrite if data is not null
        if (viewData.Data != null)
            _overridePageData = viewData.Data;
    }

    private RenderSpecs _renderSpecs;

    private RenderSpecs GetRenderSpecs()
    {
        return _renderSpecs ??= PageData.Values.FirstOrDefault(value => value is RenderSpecs) as RenderSpecs;
    }
    private object _overridePageData;

    private TypedCode16Helper CreateCodeHelper() =>
        new(
            new(ExCtx, true, Path),
            getRazorModel: () => _overridePageData
                                 // the default/only value would be on a 0 key
                                 ?? (PageData?.TryGetValue(0, out var zeroData) ?? false ? zeroData as object : null),
            () => GetRenderSpecs()?.DataDic
                  ?? PageData?
                      .Where(pair => pair.Key is string)
                      .ToDictionary(pair => pair.Key.ToString(), pair => pair.Value, InvariantCultureIgnoreCase)
        );

    #endregion


    #region Core Properties which should appear in docs

    /// <inheritdoc />
    public override ICodeLog Log => RzrHlp.CodeLog;

    /// <inheritdoc />
    public override IHtmlHelper Html => RzrHlp.Html;

    /// <inheritdoc cref="ITypedCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
        => RzrHlp.GetCode(path, noParamOrder, className);

    #endregion

    #region Link

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public ILinkService Link => CodeApi.Link;

    #endregion


    #region New App, Settings, Resources

    /// <inheritdoc />
    public new IAppTyped App => CodeApi.AppTyped;

    /// <inheritdoc cref="ITypedApi.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="ITypedApi.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion

    #region My Data Stuff

    /// <inheritdoc />
    public ITypedItem MyItem => CodeHelper.MyItem;

    /// <inheritdoc />
    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    /// <inheritdoc />
    public ITypedItem MyHeader => CodeHelper.MyHeader;

    /// <inheritdoc />
    public IDataSource MyData => CodeApi.Data;

    /// <inheritdoc />
    public ITypedRazorModel MyModel => CodeHelper.MyModel;

    #endregion


    #region MyContext & UniqueKey

    /// <inheritdoc cref="ITypedApi.MyContext" />
    public ICmsContext MyContext => CodeApi.CmsContext;

    /// <inheritdoc cref="ITypedApi.MyPage" />
    public ICmsPage MyPage => CodeApi.CmsContext.Page;

    /// <inheritdoc cref="ITypedApi.MyUser" />
    public ICmsUser MyUser => CodeApi.CmsContext.User;

    /// <inheritdoc cref="ITypedApi.MyView" />
    public ICmsView MyView => CodeApi.CmsContext.View;

    /// <inheritdoc cref="ITypedApi.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion

    #region As Conversions

    /// <inheritdoc cref="ITypedApi.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => CodeApi.Cdf.AsItem(data, new() { ItemIsStrict = propsRequired ?? true, UseMock = mock == true });

    /// <inheritdoc cref="ITypedApi.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsItems(list, new() { ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="ITypedApi.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing)
        => CodeApi.Cdf.AsEntity(thing);

    /// <inheritdoc cref="ITypedApi.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsTyped(original, new() { EntryPropIsRequired = false, ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="ITypedApi.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsTypedList(list, new() { EntryPropIsRequired = false, ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="ITypedApi.AsStack" />
    public ITypedStack AsStack(params object[] items)
        => CodeApi.Cdf.AsStack(items);

    /// <inheritdoc cref="ITypedApi.AsStack{T}" />
    public T AsStack<T>(params object[] items)
        where T : class, ICanWrapData, new()
        => CodeApi.Cdf.AsStack<T>(items);

    #endregion


    #region Dev Tools & Dev Helpers

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => CodeHelper.DevTools;

    [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpDbRazor.Compile16;

    #endregion

    #region CreateInstance

    [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

    #endregion

    #region As / AsList WIP v17

    /// <inheritdoc />
    public T As<T>(object source, NoParamOrder protector = default, bool mock = false)
        where T : class, ICanWrapData
        => CodeApi.Cdf.AsCustom<T>(source: source, protector: protector, mock: mock);

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ICanWrapData
        => CodeApi.Cdf.AsCustomList<T>(source: source, protector: protector, nullIfNull: nullIfNull);

    #endregion


    #region Experimental Configuration

    [PrivateApi("not yet public or final, WIP v20.00.0x")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    // [field: AllowNull, MaybeNull]
    public IRazorConfiguration Configuration => field ??= new RazorConfiguration(GetRenderSpecs(), RzrHlp.Log);

    #endregion
}