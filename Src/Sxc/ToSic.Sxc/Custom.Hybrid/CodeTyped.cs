using System.Runtime.CompilerServices;
using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Base class for v16 [Typed](xref:NetCode.TypedCode.Index) CSharp files.
/// Use it to create custom CS code in your App.
/// 
/// It provides the <see cref="ServiceKit16"/> on property `Kit` which contains all the popular services to create amazing stuff.
/// </summary>
/// <remarks>
/// Important: This is very different from Razor12 or Razor14, as it doesn't rely on `dynamic` code.
/// Be aware of this since the APIs are very different - see [Typed Code](xref:NetCode.TypedCode.Index).
/// </remarks>
[PublicApi]
public abstract class CodeTyped : CustomCodeBase, IHasCodeLog, IDynamicCode16
{

    #region Constructor / Setup

    /// <summary>
    /// Main constructor.
    /// Doesn't have parameters so it can easily be inherited.
    /// </summary>
    protected CodeTyped() : base("Cst.CodeTy") { }

    /// <summary>
    /// Special constructor for code files in the `ThisCode` which need the context - such as the Kit.
    /// </summary>
    /// <param name="parent"></param>
    protected CodeTyped(IHasCodeContext parent) : base("Cst.CodeTy")
    {
        if (parent is not IHasCodeApiService dynCodeParent)
            return;

        base.ConnectToRoot(dynCodeParent._CodeApiSvc);
    }

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => SysHlp.CodeLog;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeRootOrError().GetService<TService>();

    private TypedCode16Helper CodeHelper 
        => _codeHelper ??= new(CodeRootOrError(), MyData, null, false, "c# code file");
    private TypedCode16Helper _codeHelper;

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel16;

    #endregion

    /// <inheritdoc cref="IDynamicCode16.Kit"/>
    public ServiceKit16 Kit => _kit.Get(() => CodeRootOrError().GetKit<ServiceKit16>());
    private readonly GetOnce<ServiceKit16> _kit = new();

    private ICodeApiService CodeRootOrError([CallerMemberName] string propName = default)
    {
        if (_CodeApiSvc != null) return _CodeApiSvc;

        var message = $"Can't access properties such as {propName}, because the Code-Context is not known. " +
                      $"This is typical in code which is in the **ThisCode** folder. " +
                      $"Make sure the caller of the code uses 'this' in the constructor - " +
                      $"eg. 'new {GetType().Name}(this)' and that the class {GetType().Name} has a constructor which passes it to the base class " +
                      $"like public {GetType().Name}({nameof(IHasCodeContext)} parent) : base(parent) {{ }} ";
        throw new ExceptionWithHelp(new CodeHelp("get-kit-without-code-root", "todo", uiMessage: message),
            new ArgumentNullException(nameof(Kit)));
    }

    #region Stuff added by Code12

    [PrivateApi("Not yet ready")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IDevTools DevTools => CodeHelper.DevTools;

    #endregion


    #region Link and Edit
    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => CodeRootOrError()?.Link;

    #endregion


    #region SharedCode - must also map previous path to use here

    /// <inheritdoc />
    [PrivateApi]
    string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
        => SysHlp.GetCode(path, noParamOrder, className);


    #endregion


    #region New App, Settings, Resources

    /// <inheritdoc />
    public IAppTyped App => (IAppTyped)CodeRootOrError()?.App;

    /// <inheritdoc cref="IDynamicCode16.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="IDynamicCode16.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;


    public IBlockInstance MyData => CodeRootOrError().Data;

    #endregion

    #region My... Stuff

    public ITypedItem MyItem => CodeHelper.MyItem;

    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    public ITypedItem MyHeader => CodeHelper.MyHeader;

    #endregion


    #region As Conversions

    /// <inheritdoc cref="IDynamicCode16.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => CodeRootOrError()._Cdf.AsItem(data, propsRequired: propsRequired ?? true, mock: mock);

    /// <inheritdoc cref="IDynamicCode16.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeRootOrError()._Cdf.AsItems(list, propsRequired: propsRequired ?? true);

    /// <inheritdoc cref="IDynamicCode16.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing) => CodeRootOrError()._Cdf.AsEntity(thing);

    /// <inheritdoc cref="IDynamicCode16.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeRootOrError()._Cdf.AsTyped(original, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeRootOrError()._Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsStack" />
    public ITypedStack AsStack(params object[] items) => CodeRootOrError()._Cdf.AsStack(items);

    #endregion

    public ITypedModel MyModel => CodeHelper.MyModel;

    #region MyContext & UniqueKey

    /// <inheritdoc cref="IDynamicCode16.MyContext" />
    public ICmsContext MyContext => CodeRootOrError().CmsContext;

    /// <inheritdoc cref="IDynamicCode16.MyPage" />
    public ICmsPage MyPage => CodeRootOrError().CmsContext.Page;

    /// <inheritdoc cref="IDynamicCode16.MyUser" />
    public ICmsUser MyUser => CodeRootOrError().CmsContext.User;

    /// <inheritdoc cref="IDynamicCode16.MyView" />
    public ICmsView MyView => CodeRootOrError().CmsContext.View;

    /// <inheritdoc cref="IDynamicCode16.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion


    #region As / AsList WIP v17

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    protected T As<T>(ICanBeEntity source)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _CodeApiSvc._Cdf.AsCustom<T>(source, Kit);

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    protected IEnumerable<T> AsList<T>(IEnumerable<ICanBeEntity> source)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _CodeApiSvc._Cdf.AsCustomList<T>(source, Kit);

    #endregion

}