using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Context;
using System.Web.Http.Results;
using ToSic.Eav.DataSource;
using ToSic.Eav.Models;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.Compatibility;
using ToSic.Sxc.Services.Sys;
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
public abstract class ApiTyped: DnnSxcCustomControllerBase, IHasCodeLog, IDynamicWebApi, ITypedCode16, IGetCodePath
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

    /// <inheritdoc cref="IHasKit{TServiceKit}.Kit"/>
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

    /// <inheritdoc cref="ITypedCode16.GetService{TService}(NoParamOrder, string?)"/>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TService GetService<TService>(NoParamOrder npo = default, string typeName = default) where TService : class
        => AppCodeGetNamedServiceHelper.GetService<TService>(owner: this, CodeHelper.Specs, typeName);

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public ILinkService Link => CodeApi?.Link;

    [PrivateApi("Not yet ready")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public IDevTools DevTools => CodeHelper.DevTools;

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


    #region AsDynamic implementations + AsList - all killed in v16

    ///// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)" />
    //public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot.Cdf.AsDynamicFromJson(json, fallback);

    ///// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(IEntity)" />
    //public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.Cdf.AsDynamic(entity);

    ///// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(object)" />
    //public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.Cdf.AsDynamicInternal(dynamicEntity);

    ///// <inheritdoc cref="IDynamicCode12Docs.AsDynamic(object[])" />
    //public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.Cdf.MergeDynamic(entities);

    ///// <inheritdoc cref="IDynamicCodeDocs.AsEntity" />
    //public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.Cdf.AsEntity(dynamicEntity);

    ///// <inheritdoc cref="IDynamicCodeDocs.AsList" />
    //public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.Cdf.AsDynamicList(list);

    #endregion

    #region Adam

    /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
    public IFile SaveInAdam(NoParamOrder npo = default, Stream stream = null, string fileName = null, string contentType = null,
        Guid? guid = null, string field = null, string subFolder = "")
        => DynHlp.SaveInAdam(stream: stream, fileName: fileName, contentType: contentType, guid: guid, field: field, subFolder: subFolder);

    #endregion

    #region New App, Settings, Resources

    /// <inheritdoc />
    public IAppTyped App => CodeApi.AppTyped;

    /// <inheritdoc cref="ITypedApi.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="ITypedApi.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion


    #region CreateInstance

    private CompileCodeHelper CompileCodeHlp => field ??= GetService<CompileCodeHelper>().Init(this);

    string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc cref="ITypedCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder npo = default, string className = default)
        => CompileCodeHlp.GetCode(path, className: className);

    #endregion

    #region My... Stuff

    private CodeHelperTypedData CodeHelper => field
        ??= new(helperSpecs: new(ExCtx, false, ((IGetCodePath)this).CreateInstancePath));

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

    /// <inheritdoc cref="ITypedApi.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder npo = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsItem(data, new() { ItemIsStrict = propsRequired ?? true })!;

    /// <inheritdoc cref="ITypedApi.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder npo = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsItems(list, new() { ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="ITypedApi.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing)
        => CodeApi.Cdf.AsEntity(thing);

    /// <inheritdoc cref="ITypedApi.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder npo = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsTyped(original, new() { EntryPropIsRequired = false, ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="ITypedApi.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder npo = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsTypedList(list, new() { EntryPropIsRequired = false, ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="ITypedApi.AsStack" />
    public ITypedStack AsStack(params object[] items)
        => CodeApi.Cdf.AsStack(items);

    /// <inheritdoc cref="ITypedApi.AsStack{T}" />
    public T AsStack<T>(params object[] items)
        where T : class, IModelOfData, new()
        => CodeApi.Cdf.AsStack<T>(items);

    #endregion

    [PrivateApi]
    public ITypedRazorModel MyModel => throw new("MyModel isn't meant to work in WebApi");


    #region Net Core Compatibility Shims - Copy this entire section to WebApi Files

    /// <inheritdoc cref="IDynamicWebApi.File"/>
    public dynamic File(NoParamOrder npo = default,
        bool? download = null,
        string virtualPath = null,
        string contentType = null,
        string fileDownloadName = null,
        object contents = null)
        => Shim.File(download: download, virtualPath: virtualPath, contentType: contentType, fileDownloadName: fileDownloadName, contents: contents);

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
    public T As<T>(object source, NoParamOrder npo = default)
        where T : class, IModelOfData
        => CodeApi.Cdf.AsCustom<T>(source: source, npo: npo);

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder npo = default, bool nullIfNull = default)
        where T : class, IModelOfData
        => CodeApi.Cdf.AsCustomList<T>(source, npo, nullIfNull);

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