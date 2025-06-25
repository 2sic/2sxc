using System.Runtime.CompilerServices;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.Help;
using ToSic.Sys.Exceptions;

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

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => CodeHlp.CodeLog;

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class
        => CodeApi().GetService<TService>();

    [PrivateApi("WIP 17.06,x")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TService GetService<TService>(NoParamOrder protector = default, string? typeName = default) where TService : class
        => CodeHelper.GetService<TService>(protector, typeName);

    [field: AllowNull, MaybeNull]
    private TypedCode16Helper CodeHelper
        => field ??= new(owner: this, helperSpecs: new(CodeRootOrError(), false, "c# code file"), getRazorModel: () => null, getModelDic: () => null);

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel16;

    #endregion

    /// <inheritdoc cref="IDynamicCode16.Kit"/>
    [field: AllowNull, MaybeNull]
    public ServiceKit16 Kit => field ??= CodeApi().ServiceKit16;

    private ICodeTypedApiHelper CodeApi([CallerMemberName] string? propName = default)
        => _codeApi ??= CodeRootOrError(propName).GetTypedApi();
    private ICodeTypedApiHelper? _codeApi;

    private IExecutionContext CodeRootOrError([CallerMemberName] string? propName = default)
        => ExCtxOrNull
           ?? throw new ExceptionWithHelp(new CodeHelp
               {
                   Name = "get-kit-without-code-root",
                   Detect = "todo",
                   UiMessage =
                       $"Can't access properties such as {propName}, because the Code-Context is not known. " +
                       $"This is typical in code which is in the **AppCode** folder. " +
                       $"Make sure the caller of the code uses GetService<{GetType().Name}>() to create the object - " +
                       $"like 'var {GetType().Name}Svc = GetService<{GetType().Name}>()'.",
               },
#pragma warning disable CA2208
               new ArgumentNullException(nameof(Kit)));
#pragma warning restore CA2208


    #region Stuff added by Code12

    [PrivateApi("Not yet ready")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public IDevTools DevTools => CodeHelper.DevTools;

    #endregion


    #region Link and Edit
    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => CodeApi().Link;

    #endregion


    #region SharedCode - must also map previous path to use here

    /// <inheritdoc />
    [PrivateApi]
    string IGetCodePath.CreateInstancePath { get; set; } = null!;

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    public dynamic? GetCode(string path, NoParamOrder noParamOrder = default, string? className = default)
        => CodeHlp.GetCode(path: path, className: className);


    #endregion


    #region New App, Settings, Resources

    /// <inheritdoc />
    public IAppTyped App => CodeApi().AppTyped;

    /// <inheritdoc cref="IDynamicCode16.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="IDynamicCode16.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;


    public IDataSource MyData => CodeApi().Data;

    #endregion

    #region My... Stuff

    public ITypedItem MyItem => CodeHelper.MyItem;

    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    public ITypedItem MyHeader => CodeHelper.MyHeader;

    #endregion


    #region As Conversions

    /// <inheritdoc cref="IDynamicCode16.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => CodeApi().Cdf.AsItem(data, new() { ItemIsStrict = propsRequired ?? true })!;

    /// <inheritdoc cref="IDynamicCode16.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi().Cdf.AsItems(list, new() { ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="IDynamicCode16.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing)
        => CodeApi().Cdf.AsEntity(thing);

    /// <inheritdoc cref="IDynamicCode16.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi().Cdf.AsTyped(original, new() { FirstIsRequired = false, ItemIsStrict = propsRequired ?? true })!;

    /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi().Cdf.AsTypedList(list, new() { FirstIsRequired = false, ItemIsStrict = propsRequired ?? true })!;

    /// <inheritdoc cref="IDynamicCode16.AsStack" />
    public ITypedStack AsStack(params object[] items)
        => CodeApi().Cdf.AsStack(items);

    /// <inheritdoc cref="IDynamicCode16.AsStack{T}" />
    public T AsStack<T>(params object[] items)
        where T : class, ICanWrapData, new()
        => CodeApi().Cdf.AsStack<T>(items);

    #endregion

    public ITypedRazorModel MyModel => CodeHelper.MyModel;

    #region MyContext & UniqueKey

    /// <inheritdoc cref="IDynamicCode16.MyContext" />
    public ICmsContext MyContext => CodeApi().CmsContext;

    /// <inheritdoc cref="IDynamicCode16.MyPage" />
    public ICmsPage MyPage => CodeApi().CmsContext.Page;

    /// <inheritdoc cref="IDynamicCode16.MyUser" />
    public ICmsUser MyUser => CodeApi().CmsContext.User;

    /// <inheritdoc cref="IDynamicCode16.MyView" />
    public ICmsView MyView => CodeApi().CmsContext.View;

    /// <inheritdoc cref="IDynamicCode16.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion


    #region As / AsList WIP v17

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    /// <inheritdoc />
    public T As<T>(object source, NoParamOrder protector = default, bool mock = false)
        where T : class, ICanWrapData
        => Cdf.AsCustom<T>(source: source, protector: protector, mock: mock)!;

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ICanWrapData
        => Cdf.AsCustomList<T>(source, protector, nullIfNull);

    #endregion

    #region Customize

    /// <summary>
    /// Helper to create typed objects for App, View etc. - mainly for custom base classes in `AppCode`
    /// </summary>
    /// <remarks>
    /// * Introduced in v17.03 (beta)
    /// * Stable and ready for production in v18.00
    /// </remarks>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    [field: AllowNull, MaybeNull]
    protected ICodeCustomizer Customize
        => field ??= ExCtx.GetService<ICodeCustomizer>(reuse: true);

    #endregion

}