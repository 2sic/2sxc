using ToSic.Eav.Code.Help;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Razor;
using ToSic.Sxc.Dnn.Razor.Internal;
using ToSic.Sxc.Engines;
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
public abstract class RazorTyped: RazorComponentBase, IRazor, IDynamicCode16, IHasCodeHelp, IGetCodePath, ISetDynamicModel, ICanUseRoslynCompiler
{
    #region Constructor, Setup, Helpers

    /// <inheritdoc cref="DnnRazorHelper.RenderPageNotSupported"/>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override HelperResult RenderPage(string path, params object[] data)
        => RzrHlp.RenderPageNotSupported();


    /// <inheritdoc cref="ICompatibilityLevel.CompatibilityLevel"/>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel16;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => _CodeApiSvc.GetService<TService>();

    [PrivateApi("WIP 17.06,x")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public TService GetService<TService>(NoParamOrder protector = default, string typeName = default) where TService : class
        => CodeHelper.GetService<TService>(protector, typeName);

    /// <inheritdoc cref="IDynamicCode16.Kit"/>
    public ServiceKit16 Kit => _kit.Get(_CodeApiSvc.GetKit<ServiceKit16>);
    private readonly GetOnce<ServiceKit16> _kit = new();

    internal TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
    private TypedCode16Helper _codeHelper;

    /// <summary>
    /// BETA! v17.03+
    /// </summary>
    [WorkInProgressApi("Experiment v17.03+")]
    protected ICodeCustomizer Customize => _customize ??= _CodeApiSvc.GetService<ICodeCustomizer>(reuse: true);
    private ICodeCustomizer _customize;

    void ISetDynamicModel.SetDynamicModel(object data) => _overridePageData = data;

    private object _overridePageData;

    private TypedCode16Helper CreateCodeHelper() =>
        new(
            owner: this,
            new(_CodeApiSvc, true, Path),
            getRazorModel: () => _overridePageData
                                 // the default/only value would be on a 0 key
                                 ?? (PageData?.TryGetValue(0, out var zeroData) ?? false ? zeroData as object : null),
            () => _overridePageData?.ToDicInvariantInsensitive()
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

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
        => RzrHlp.GetCode(path, noParamOrder, className);

    #endregion

    #region Link, Edit

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _CodeApiSvc.Link;

    #endregion


    #region New App, Settings, Resources

    /// <inheritdoc />
    public new IAppTyped App => _CodeApiSvc.AppTyped;

    /// <inheritdoc cref="IDynamicCode16.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="IDynamicCode16.AllSettings" />
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
    public IBlockInstance MyData => _CodeApiSvc.Data;

    /// <inheritdoc />
    public ITypedModel MyModel => CodeHelper.MyModel;

    #endregion


    #region MyContext & UniqueKey

    /// <inheritdoc cref="IDynamicCode16.MyContext" />
    public ICmsContext MyContext => _CodeApiSvc.CmsContext;

    /// <inheritdoc cref="IDynamicCode16.MyPage" />
    public ICmsPage MyPage => _CodeApiSvc.CmsContext.Page;

    /// <inheritdoc cref="IDynamicCode16.MyUser" />
    public ICmsUser MyUser => _CodeApiSvc.CmsContext.User;

    /// <inheritdoc cref="IDynamicCode16.MyView" />
    public ICmsView MyView => _CodeApiSvc.CmsContext.View;

    /// <inheritdoc cref="IDynamicCode16.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion

    #region As Conversions

    /// <inheritdoc cref="IDynamicCode16.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => _CodeApiSvc.Cdf.AsItem(data, propsRequired: propsRequired ?? true, mock: mock);

    /// <inheritdoc cref="IDynamicCode16.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default) 
        => _CodeApiSvc.Cdf.AsItems(list, propsRequired: propsRequired ?? true);

    /// <inheritdoc cref="IDynamicCode16.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing)
        => _CodeApiSvc.Cdf.AsEntity(thing);

    /// <inheritdoc cref="IDynamicCode16.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _CodeApiSvc.Cdf.AsTyped(original, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _CodeApiSvc.Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsStack" />
    public ITypedStack AsStack(params object[] items)
        => _CodeApiSvc.Cdf.AsStack(items);

    /// <inheritdoc cref="IDynamicCode16.AsStack{T}" />
    public T AsStack<T>(params object[] items)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _CodeApiSvc.Cdf.AsStack<T>(items);

    #endregion


    #region Dev Tools & Dev Helpers

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => CodeHelper.DevTools;

    [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpForRazorTyped.Compile16;

    #endregion

    #region CreateInstance

    [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

    #endregion

    #region As / AsList WIP v17

    /// <inheritdoc />
    public T As<T>(object source, NoParamOrder protector = default, bool mock = false)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _CodeApiSvc.Cdf.AsCustom<T>(source: source, protector: protector, mock: mock);

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _CodeApiSvc.Cdf.AsCustomList<T>(source: source, protector: protector, nullIfNull: nullIfNull);

    #endregion

}