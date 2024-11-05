using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Eav.WebApi;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Custom;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Custom base controller class for custom dynamic 2sxc app api controllers.
/// It is without dependencies in class constructor, commonly provided with DI.
/// </summary>
[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
public abstract class Api12(string logSuffix) : OqtStatefulControllerBase(logSuffix), IDynamicWebApi, IDynamicCode12,
    IHasCodeLog, IHasCodeApiService
{
    #region Setup

    protected Api12() : this(EavWebApiConstants.HistoryNameWebApi) { }

    [PrivateApi] public int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel12;

    /// <summary>
    /// Our custom dynamic 2sxc app api controllers, depends on event OnActionExecuting to provide dependencies (without DI in constructor).
    /// </summary>
    /// <param name="context"></param>
    [NonAction]
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        CtxHlp.OnActionExecutingEnd(context);
    }

    #endregion

    #region Infrastructure

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => CtxHlp.CodeLog;

    // ReSharper disable once InconsistentNaming
    [PrivateApi] public ICodeApiService _CodeApiSvc => CtxHlp._CodeApiSvc;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public new TService GetService<TService>() where TService : class => _CodeApiSvc.GetService<TService>();

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => _CodeApiSvc.DevTools;

    #endregion

    #region App, Data, Settings, Resources, CmsContext

    /// <inheritdoc cref="IDynamicCode.App" />
    public IApp App => _CodeApiSvc?.App;

    /// <inheritdoc cref="IDynamicCode.Data" />
    public IBlockInstance Data => _CodeApiSvc?.Data;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => _CodeApiSvc.Resources;

    /// <inheritdoc cref="IDynamicCode12.Settings" />
    public dynamic Settings => _CodeApiSvc.Settings;

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => _CodeApiSvc?.CmsContext;


    #endregion


    #region AsDynamic / AsList implementations

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    [NonAction]
    public dynamic AsDynamic(string json, string fallback = default) => _CodeApiSvc.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    [NonAction]
    public dynamic AsDynamic(IEntity entity) => _CodeApiSvc.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    [NonAction]
    public dynamic AsDynamic(object dynamicEntity) => _CodeApiSvc.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    [NonAction]
    public dynamic AsDynamic(params object[] entities) => _CodeApiSvc.Cdf.MergeDynamic(entities);

    /// <inheritdoc cref="IDynamicCode.AsList" />
    [NonAction]
    public IEnumerable<dynamic> AsList(object list) => _CodeApiSvc.Cdf.CodeAsDynList(list);

    #endregion

    #region AsEntity

    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => _CodeApiSvc?.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region Convert-Service
    [PrivateApi] public IConvertService Convert => _CodeApiSvc.Convert;

    #endregion


    #region CreateSource implementations

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    [NonAction]
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => _CodeApiSvc.CreateSource<T>(inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    [NonAction]
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => _CodeApiSvc.CreateSource<T>(source);

    #endregion

    #region Content, Presentation & List

    /// <inheritdoc cref="IDynamicCode.Content" />
    public new dynamic Content => _CodeApiSvc?.Content;

    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => _CodeApiSvc?.Header;


    #endregion

    #region Adam

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    [NonAction]
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => _CodeApiSvc?.AsAdam(item, fieldName);

    /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
    [NonAction]
    public IFile SaveInAdam(NoParamOrder noParamOrder = default,
        Stream stream = null,
        string fileName = null,
        string contentType = null,
        Guid? guid = null,
        string field = null,
        string subFolder = "")
        => CtxHlp.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

    #endregion

    #region Link & Edit - added to API in 2sxc 10.01

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _CodeApiSvc?.Link;

    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => _CodeApiSvc?.Edit;

    #endregion

    #region  CreateInstance implementation

    string IGetCodePath.CreateInstancePath { get; set; }

    protected CodeHelper CodeHlp => _codeHlp ??= GetService<CodeHelper>().Init(this);
    private CodeHelper _codeHlp;

    /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
    [NonAction]
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
        => CodeHlp.CreateInstance(virtualPath: virtualPath, name: name, throwOnError: throwOnError);

    #endregion

    #region File Response / Download

    /// <inheritdoc cref="IDynamicWebApi.File"/>
    public dynamic File(NoParamOrder noParamOrder = default,
        bool? download = null,
        string virtualPath = null,
        string contentType = null,
        string fileDownloadName = null,
        object contents = null
    ) =>
        new OqtWebApiShim(response: Response, this).File(noParamOrder, download, virtualPath, contentType, fileDownloadName, contents);

    #endregion

}