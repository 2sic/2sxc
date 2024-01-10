using System.Collections.Generic;
using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;
using System.Web.Http;
using ToSic.Sxc.Context;
using System.IO;
using System.Net.Http;
using System.Web.Http.Results;
using ToSic.Eav.WebApi;
using ToSic.Lib.Coding;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Internal;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Base class for v14 Dynamic WebAPI files.
/// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
/// This contains all the popular services used in v14, so that your code can be lighter. 
/// </summary>
/// <remarks>
/// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
/// </remarks>
[PublicApi]
[DnnLogExceptions]
//[DefaultToNewtonsoftForHttpJson] - // !!! v16 should now default to normal
[JsonFormatter]
public abstract class ApiTyped: DynamicApiController, IHasCodeLog, IDynamicWebApi, IHasDynamicCodeRoot, IDynamicCode16, IGetCodePath
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

    /// <inheritdoc cref="IDynamicCodeKit{ServiceKit14}.Kit" />
    /// <inheritdoc cref="IDynamicCode16.Kit"/>
    public ServiceKit16 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit16>());
    private readonly GetOnce<ServiceKit16> _kit = new();

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => SysHlp.CodeLog;

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel16;

    #endregion

    #region Link & Edit - added to API in 2sxc 10.01; CmsContext, Resources, Settings (v12)

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => SysHlp.GetService<TService>();

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _DynCodeRoot?.Link;

    [PrivateApi("Not yet ready")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IDevTools DevTools => CodeHelper.DevTools;

    #endregion

    #region MyContext & UniqueKey

    /// <inheritdoc cref="IDynamicCode16.MyContext" />
    public ICmsContext MyContext => _DynCodeRoot.CmsContext;

    /// <inheritdoc cref="IDynamicCode16.MyPage" />
    public ICmsPage MyPage => _DynCodeRoot.CmsContext.Page;

    /// <inheritdoc cref="IDynamicCode16.MyUser" />
    public ICmsUser MyUser => _DynCodeRoot.CmsContext.User;

    /// <inheritdoc cref="IDynamicCode16.MyView" />
    public ICmsView MyView => _DynCodeRoot.CmsContext.View;

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

    ///// <inheritdoc cref="IDynamicCode.AsAdam" />
    //public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot.AsAdam(item, fieldName);

    /// <inheritdoc cref="IDynamicWebApi.SaveInAdam" />
    public new ToSic.Sxc.Adam.IFile SaveInAdam(NoParamOrder noParamOrder = default,
        Stream stream = null,
        string fileName = null,
        string contentType = null,
        Guid? guid = null,
        string field = null,
        string subFolder = "")
        => base.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

    #endregion

    #region New App, Settings, Resources

    /// <inheritdoc />
    public IAppTyped App => (IAppTyped)_DynCodeRoot.App;

    /// <inheritdoc cref="IDynamicCode16.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="IDynamicCode16.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion


    #region CreateInstance

    string IGetCodePath.CreateInstancePath { get; set; }


    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default) 
        => _DynCodeRoot.CreateInstance(path, relativePath: ((IGetCodePath)this).CreateInstancePath, name: className);

    #endregion

    #region My... Stuff

    private TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
    private TypedCode16Helper _codeHelper;

    private TypedCode16Helper CreateCodeHelper()
    {
        return new TypedCode16Helper(_DynCodeRoot, MyData, null, false, "c# WebApiController");
    }

    public ITypedItem MyItem => CodeHelper.MyItem;

    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    public ITypedItem MyHeader => CodeHelper.MyHeader;

    public IBlockRun MyData => _DynCodeRoot.Data;

    #endregion


    #region As Conversions

    /// <inheritdoc cref="IDynamicCode16.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => _DynCodeRoot.Cdf.AsItem(data, propsRequired: propsRequired ?? true, mock: mock);

    /// <inheritdoc cref="IDynamicCode16.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _DynCodeRoot.Cdf.AsItems(list, propsRequired: propsRequired ?? true);

    /// <inheritdoc cref="IDynamicCode16.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.Cdf.AsEntity(thing);

    /// <inheritdoc cref="IDynamicCode16.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _DynCodeRoot.Cdf.AsTyped(original, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _DynCodeRoot.Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsStack" />
    public ITypedStack AsStack(params object[] items) => _DynCodeRoot.Cdf.AsStack(items);

    #endregion

    public ITypedModel MyModel => CodeHelper.MyModel;


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

    // TODO: this Shim could now be implemented after 16.02 - since we don't have the Content property any more
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

}