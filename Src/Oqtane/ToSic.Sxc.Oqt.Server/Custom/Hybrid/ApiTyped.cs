using System.Collections.Generic;
using System;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.Oqt.Server.Custom;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using ToSic.Sxc.Adam;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Lib.Coding;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Internal;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Oqtane specific Api base class.
///
/// It's identical to [](xref:Custom.Hybrid.Api14) but this may be enhanced in future. 
/// </summary>
[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
[JsonFormatter]
public abstract class ApiTyped(string logSuffix) : OqtStatefulControllerBase(logSuffix), IDynamicWebApi, IHasCodeLog,
    IHasCodeApiService, IDynamicCode16
{
    #region setup

    protected ApiTyped() : this(EavWebApiConstants.HistoryNameWebApi) { }

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

    [PrivateApi] public int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel16;

    #endregion

    #region Infrastructure

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => CtxHlp.CodeLog;

    // ReSharper disable once InconsistentNaming
    [PrivateApi] public ICodeApiService _CodeApiSvc => CtxHlp._CodeApiSvc;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public new TService GetService<TService>() where TService : class => _CodeApiSvc.GetService<TService>();

    /// <inheritdoc cref="IDynamicCode16.Kit"/>
    public ServiceKit16 Kit => _kit.Get(() => _CodeApiSvc.GetKit<ServiceKit16>());
    private readonly GetOnce<ServiceKit16> _kit = new();

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => CodeHelper.DevTools;

    #endregion

    #region Link & Edit - added to API in 2sxc 10.01

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _CodeApiSvc?.Link;

    #endregion


    #region Adam

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


    #region New App, Settings, Resources

    /// <inheritdoc />
    public IAppTyped App => (IAppTyped)_CodeApiSvc?.App;

    /// <inheritdoc cref="IDynamicCode16.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="IDynamicCode16.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion

    #region My... Stuff

    private TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
    private TypedCode16Helper _codeHelper;

    private TypedCode16Helper CreateCodeHelper() => new(_CodeApiSvc, MyData, null, false, "c# WebApiController");

    public ITypedItem MyItem => CodeHelper.MyItem;

    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    public ITypedItem MyHeader => CodeHelper.MyHeader;

    public IBlockInstance MyData => _CodeApiSvc.Data;

    #endregion


    #region As Conversions

    /// <inheritdoc cref="IDynamicCode16.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => _CodeApiSvc._Cdf.AsItem(data, propsRequired: propsRequired ?? true, mock: mock);

    /// <inheritdoc cref="IDynamicCode16.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _CodeApiSvc._Cdf.AsItems(list, propsRequired: propsRequired ?? true);

    /// <inheritdoc cref="IDynamicCode16.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing) => _CodeApiSvc._Cdf.AsEntity(thing);

    /// <inheritdoc cref="IDynamicCode16.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _CodeApiSvc._Cdf.AsTyped(original, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _CodeApiSvc._Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsStack" />
    public ITypedStack AsStack(params object[] items) => _CodeApiSvc._Cdf.AsStack(items);

    #endregion

    public ITypedModel MyModel => CodeHelper.MyModel;

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
    {
        return _CodeApiSvc.CreateInstance(path, relativePath: (this as IGetCodePath).CreateInstancePath, name: className);
    }

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

    #region  CreateInstance implementation

    string IGetCodePath.CreateInstancePath { get; set; }

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