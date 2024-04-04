using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.WebApi.Internal.Compatibility;
using ToSic.Sxc.Dnn.WebApi.Internal.HttpJson;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// This is the base class for all custom API Controllers. <br/>
/// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
/// </summary>
[PublicApi("This is the official base class for v12+")]
[DnnLogExceptions]
[DefaultToNewtonsoftForHttpJson]
public abstract partial class Api12: DnnSxcCustomControllerBase, IDynamicCode12, IDynamicWebApi, IHasCodeLog, ICreateInstance
{
    #region Setup

    protected Api12() : base("Hyb12") { }

    protected Api12(string logSuffix) : base(logSuffix) { }

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => SysHlp.CodeLog;

    [PrivateApi] public int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel12;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => SysHlp.GetService<TService>();

    #endregion

    #region Content, Presentation, Header, App, Data

    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => _CodeApiSvc.Content;

    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => _CodeApiSvc.Header;

    /// <inheritdoc cref="IDynamicCode.App" />
    public IApp App => _CodeApiSvc.App;

    /// <inheritdoc cref="IDynamicCode.Data" />
    public IBlockInstance Data => _CodeApiSvc.Data;

    #endregion


    #region Link & Edit - added to API in 2sxc 10.01; CmsContext, Resources, Settings (v12)

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _CodeApiSvc?.Link;

    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => _CodeApiSvc?.Edit;

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => _CodeApiSvc?.CmsContext;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => _CodeApiSvc.Resources;

    /// <inheritdoc cref="IDynamicCode12.Settings" />
    public dynamic Settings => _CodeApiSvc.Settings;

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => _CodeApiSvc.DevTools;

    #endregion


    #region AsDynamic implementations + AsList

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => _CodeApiSvc.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => _CodeApiSvc.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    public dynamic AsDynamic(object dynamicEntity) => _CodeApiSvc.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    public dynamic AsDynamic(params object[] entities) => _CodeApiSvc.Cdf.MergeDynamic(entities);

    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => _CodeApiSvc.Cdf.AsEntity(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode.AsList" />
    public IEnumerable<dynamic> AsList(object list) => _CodeApiSvc?.Cdf.CodeAsDynList(list);

    #endregion


    #region Convert-Service

    /// <inheritdoc cref="IDynamicCode12.Convert" />
    public IConvertService Convert => _CodeApiSvc.Convert;

    #endregion


    #region CreateSource implementations

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => _CodeApiSvc.CreateSource<T>(source);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => _CodeApiSvc.CreateSource<T>(inSource, configurationProvider);

    #endregion


    #region Adam

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => _CodeApiSvc.AsAdam(item, fieldName);

    /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
    public IFile SaveInAdam(NoParamOrder noParamOrder = default, Stream stream = null, string fileName = null, string contentType = null,
        Guid? guid = null, string field = null, string subFolder = "")
        => DynHlp.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

    #endregion

    #region CreateInstance

    string IGetCodePath.CreateInstancePath { get; set; }

    private CodeHelper CodeHlp => _codeHlp ??= GetService<CodeHelper>().Init(this);
    private CodeHelper _codeHlp;

    /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
        => CodeHlp.CreateInstance(virtualPath: virtualPath, name: name, throwOnError: throwOnError);

    #endregion



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
    [NonAction] public new dynamic Ok() => Shim.Ok();

    /// <inheritdoc cref="WebApiCoreShim.Ok(object)"/>
    [NonAction] public dynamic Ok(object value) => Shim.Ok(value);

    /// <inheritdoc cref="WebApiCoreShim.NoContent()"/>
    [NonAction]
    public dynamic NoContent() => Shim.NoContent();

    // TODO: this Shim could now be implemented after 16.02 - since we don't have the Content property any more
    #region Content (ca. 5 overloads) can't be implemented, because it conflicts with our property "Content"

    #endregion

    /// <inheritdoc cref="WebApiCoreShim.Redirect"/>
    [NonAction] public new dynamic Redirect(string url) => Shim.Redirect(url);

    /// <inheritdoc cref="WebApiCoreShim.RedirectPermanent"/>
    [NonAction] public dynamic RedirectPermanent(string url) => Shim.RedirectPermanent(url);


    /// <inheritdoc cref="WebApiCoreShim.StatusCode(int)"/>
    [NonAction] public dynamic StatusCode(int statusCode) => Shim.StatusCode(statusCode);

    /// <inheritdoc cref="WebApiCoreShim.StatusCode(int, object)"/>
    [NonAction] public dynamic StatusCode(int statusCode, object value) => Shim.StatusCode(statusCode, value);


    /// <inheritdoc cref="WebApiCoreShim.Unauthorized()"/>
    [NonAction] public dynamic Unauthorized() => Shim.Unauthorized();

    /// <inheritdoc cref="WebApiCoreShim.Unauthorized(object)"/>
    [NonAction] public dynamic Unauthorized(object value) => Shim.Unauthorized(value);

    /// <inheritdoc cref="WebApiCoreShim.NotFound()"/>
    [NonAction] public new dynamic NotFound() => Shim.NotFound();

    /// <inheritdoc cref="WebApiCoreShim.NotFound(object)"/>
    [NonAction] public dynamic NotFound(object value) => Shim.NotFound(value);

    /// <inheritdoc cref="WebApiCoreShim.BadRequest()"/>
    [NonAction] public new dynamic BadRequest() => Shim.BadRequest();

    /// <inheritdoc cref="WebApiCoreShim.Conflict()"/>
    [NonAction] public new dynamic Conflict() => Shim.Conflict();

    /// <inheritdoc cref="WebApiCoreShim.Conflict(object)"/>
    [NonAction] public dynamic Conflict(object error) => Shim.Conflict(error);

    /// <inheritdoc cref="WebApiCoreShim.Accepted()"/>
    [NonAction] public dynamic Accepted() => Shim.Accepted();

    /// <inheritdoc cref="WebApiCoreShim.Forbid()"/>
    [NonAction] public dynamic Forbid() => Shim.Forbid();

    #endregion

}