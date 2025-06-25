using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Context;
using System.Web.Http.Results;
using ToSic.Eav.DataSource;
using ToSic.Eav.WebApi.Sys;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.CodeApi.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Dnn.WebApi.Internal.Compatibility;
using ToSic.Sxc.Sys.ExecutionContext;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Base class for v16 [Typed](xref:NetCode.TypedCode.Index) WebAPI files.
/// Use it to create custom WebAPI endpoints in your App.
/// 
/// It provides the <see cref="ServiceKit16"/> on property `Kit` which contains all the popular services to create amazing stuff.
/// </summary>
/// <remarks>
/// Important: This is very different from Razor12 or Razor14, as it doesn't rely on `dynamic` code.
/// Be aware of this since the APIs are very different - see [Typed Code](xref:NetCode.TypedCode.Index).
/// </remarks>
[PublicApi]
[DnnLogExceptions]
//[DefaultToNewtonsoftForHttpJson] - // !!! v16 should now default to normal
[JsonFormatter]
public abstract class ApiTyped: DnnSxcCustomControllerBase, IHasCodeLog, IDynamicWebApi, IDynamicCode16, IGetCodePath
{
    #region Setup

    /// <summary>
    /// Main constructor.
    /// Doesn't have parameters so it can easily be inherited.
    /// </summary>
    protected ApiTyped() : base(EavWebApiConstants.HistoryNameWebApi) { }

    /// <summary>
    /// Alternate constructor to use when inheriting, placing the Insights logs in an own section.
    /// </summary>
    /// <param name="insightsGroup">Name of the section in Insights</param>
    protected ApiTyped(string insightsGroup) : base("Api16", insightsGroup) { }

    internal ICodeTypedApiHelper CodeApi => field
        ??= ExCtx.GetTypedApi();

    /// <inheritdoc cref="IHasKit{TServiceKit}.Kit" />
    /// <inheritdoc cref="IDynamicCode16.Kit"/>
    public ServiceKit16 Kit => field ??= CodeApi.ServiceKit16;

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => SysHlp.CodeLog;

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel16;

    #endregion

    #region Link & Edit - added to API in 2sxc 10.01; CmsContext, Resources, Settings (v12)

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => SysHlp.GetService<TService>();

    [PrivateApi("WIP 17.06,x")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public TService GetService<TService>(NoParamOrder protector = default, string typeName = default) where TService : class
        => CodeHelper.GetService<TService>(protector, typeName);

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => CodeApi?.Link;

    [PrivateApi("Not yet ready")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public IDevTools DevTools => CodeHelper.DevTools;

    #endregion

    #region MyContext & UniqueKey

    /// <inheritdoc cref="IDynamicCode16.MyContext" />
    public ICmsContext MyContext => CodeApi.CmsContext;

    /// <inheritdoc cref="IDynamicCode16.MyPage" />
    public ICmsPage MyPage => CodeApi.CmsContext.Page;

    /// <inheritdoc cref="IDynamicCode16.MyUser" />
    public ICmsUser MyUser => CodeApi.CmsContext.User;

    /// <inheritdoc cref="IDynamicCode16.MyView" />
    public ICmsView MyView => CodeApi.CmsContext.View;

    /// <inheritdoc cref="IDynamicCode16.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion


    #region AsDynamic implementations + AsList - all killed in v16

    ///// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    //public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot.Cdf.AsDynamicFromJson(json, fallback);

    ///// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    //public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.Cdf.AsDynamic(entity);

    ///// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    //public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.Cdf.AsDynamicInternal(dynamicEntity);

    ///// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    //public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.Cdf.MergeDynamic(entities);

    ///// <inheritdoc cref="IDynamicCode.AsEntity" />
    //public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.Cdf.AsEntity(dynamicEntity);

    ///// <inheritdoc cref="IDynamicCode.AsList" />
    //public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.Cdf.AsDynamicList(list);

    #endregion

    #region Adam

    /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
    public IFile SaveInAdam(NoParamOrder noParamOrder = default, Stream stream = null, string fileName = null, string contentType = null,
        Guid? guid = null, string field = null, string subFolder = "")
        => DynHlp.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

    #endregion

    #region New App, Settings, Resources

    /// <inheritdoc />
    public IAppTyped App => CodeApi.AppTyped;

    /// <inheritdoc cref="IDynamicCode16.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="IDynamicCode16.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion


    #region CreateInstance

    private CodeHelper CodeHlp => field ??= GetService<CodeHelper>().Init(this);

    string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
        => CodeHlp.GetCode(path, className: className);

    #endregion

    #region My... Stuff

    private TypedCode16Helper CodeHelper => field ??= CreateCodeHelper();

    private TypedCode16Helper CreateCodeHelper()
    {
        // Create basic helper without any RazorModels, since that doesn't exist here
        return new(owner: this, helperSpecs: new(ExCtx, false, ((IGetCodePath)this).CreateInstancePath), getRazorModel: () => null, getModelDic: () => null);
    }

    /// <inheritdoc />
    public ITypedItem MyItem => CodeHelper.MyItem;

    /// <inheritdoc />
    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    /// <inheritdoc />
    public ITypedItem MyHeader => CodeHelper.MyHeader;

    /// <inheritdoc />
    public IDataSource MyData => CodeApi.Data;

    #endregion


    #region As Conversions

    /// <inheritdoc cref="IDynamicCode16.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => CodeApi.Cdf.AsItem(data, new() { ItemIsStrict = propsRequired ?? true, UseMock = mock == true });

    /// <inheritdoc cref="IDynamicCode16.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsItems(list, new() { ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="IDynamicCode16.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing)
        => CodeApi.Cdf.AsEntity(thing);

    /// <inheritdoc cref="IDynamicCode16.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsTyped(original, new() { FirstIsRequired = false, ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsTypedList(list, new() { FirstIsRequired = false, ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="IDynamicCode16.AsStack" />
    public ITypedStack AsStack(params object[] items)
        => CodeApi.Cdf.AsStack(items);

    /// <inheritdoc cref="IDynamicCode16.AsStack{T}" />
    public T AsStack<T>(params object[] items)
        where T : class, ICanWrapData, new()
        => CodeApi.Cdf.AsStack<T>(items);

    #endregion

    /// <inheritdoc />
    public ITypedRazorModel MyModel => CodeHelper.MyModel;


    #region Net Core Compatibility Shims - Copy this entire section to WebApi Files

    /// <inheritdoc cref="IDynamicWebApi.File"/>
    public dynamic File(NoParamOrder noParamOrder = default,
        bool? download = null,
        string virtualPath = null,
        string contentType = null,
        string fileDownloadName = null,
        object contents = null)
        => Shim.File(noParamOrder, download, virtualPath, contentType, fileDownloadName, contents);

    private WebApiCoreShim Shim => new(Request);

    /// <inheritdoc cref="WebApiCoreShim.Ok()"/>
    [NonAction]
    public new OkResult Ok() => base.Ok(); // Shim.Ok();

    /// <inheritdoc cref="WebApiCoreShim.Ok(object)"/>
    [NonAction] public HttpResponseMessage Ok(object value) => Shim.Ok(value);

    /// <inheritdoc cref="WebApiCoreShim.NoContent()"/>
    [NonAction]
    public HttpResponseMessage NoContent() => Shim.NoContent();

    // TODO: the Content Shim could now be implemented after 16.02 - since we don't have the Content property any more

    #region Content (ca. 5 overloads) can't be implemented, because it conflicts with our property "Content"

    #endregion

    /// <inheritdoc cref="WebApiCoreShim.Redirect"/>
    [NonAction]
    public new RedirectResult Redirect(string url) => base.Redirect(url); // Shim.Redirect(url);

    /// <inheritdoc cref="WebApiCoreShim.RedirectPermanent"/>
    [NonAction] public HttpResponseMessage RedirectPermanent(string url) => Shim.RedirectPermanent(url);


    /// <inheritdoc cref="WebApiCoreShim.StatusCode(int)"/>
    [NonAction] public HttpResponseMessage StatusCode(int statusCode) => Shim.StatusCode(statusCode);

    /// <inheritdoc cref="WebApiCoreShim.StatusCode(int, object)"/>
    [NonAction] public HttpResponseMessage StatusCode(int statusCode, object value) => Shim.StatusCode(statusCode, value);


    /// <inheritdoc cref="WebApiCoreShim.Unauthorized()"/>
    [NonAction] public HttpResponseMessage Unauthorized() => Shim.Unauthorized();

    /// <inheritdoc cref="WebApiCoreShim.Unauthorized(object)"/>
    [NonAction] public HttpResponseMessage Unauthorized(object value) => Shim.Unauthorized(value);

    /// <inheritdoc cref="WebApiCoreShim.NotFound()"/>
    [NonAction]
    public new NotFoundResult NotFound() => base.NotFound();// Shim.NotFound();

    /// <inheritdoc cref="WebApiCoreShim.NotFound(object)"/>
    [NonAction] public HttpResponseMessage NotFound(object value) => Shim.NotFound(value);

    /// <inheritdoc cref="WebApiCoreShim.BadRequest()"/>
    [NonAction]
    public new BadRequestResult BadRequest() => base.BadRequest(); // Shim.BadRequest();

    /// <inheritdoc cref="WebApiCoreShim.Conflict()"/>
    [NonAction]
    public new ConflictResult Conflict() => base.Conflict(); // Shim.Conflict();

    /// <inheritdoc cref="WebApiCoreShim.Conflict(object)"/>
    [NonAction] public HttpResponseMessage Conflict(object error) => Shim.Conflict(error);

    /// <inheritdoc cref="WebApiCoreShim.Accepted()"/>
    [NonAction] public HttpResponseMessage Accepted() => Shim.Accepted();

    /// <inheritdoc cref="WebApiCoreShim.Forbid()"/>
    [NonAction] public HttpResponseMessage Forbid() => Shim.Forbid();

    #endregion

    #region As / AsList WIP v17

    /// <inheritdoc />
    public T As<T>(object source, NoParamOrder protector = default, bool mock = false)
        where T : class, ICanWrapData
        => CodeApi.Cdf.AsCustom<T>(source: source, protector: protector, mock: mock);

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ICanWrapData
        => CodeApi.Cdf.AsCustomList<T>(source, protector, nullIfNull);

    #endregion

    #region Customize new WIP v17

    /// <summary>
    /// WIP
    /// </summary>
    [PrivateApi("Experiment v17.02+")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    protected ICodeCustomizer Customize => field ??= CodeApi.GetService<ICodeCustomizer>(reuse: true);

    #endregion

}