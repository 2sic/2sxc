using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.Oqt.Server.Custom;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Adam;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Oqtane specific Api base class.
///
/// It's identical to [](xref:Custom.Hybrid.Api14) but this may be enhanced in future. 
/// </summary>
[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
[JsonFormatter]
public abstract class ApiTyped(string logSuffix) : OqtStatefulControllerBase(logSuffix), IDynamicWebApi, IHasCodeLog, ITypedCode16
{
    #region setup

    protected ApiTyped() : this(EavWebApiConstants.HistoryNameWebApi) { }

    internal ICodeTypedApiHelper CodeApi => field
        ??= ExCtxOrNull.GetTypedApi();


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
    [PrivateApi] internal IExecutionContext ExCtxOrNull => CtxHlp.ExCtxOrNull;

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public new TService GetService<TService>() where TService : class
        => CodeApi.GetService<TService>();

    /// <inheritdoc cref="ITypedCode16.GetService{TService}(NoParamOrder, string?)"/>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TService GetService<TService>(NoParamOrder npo = default, string typeName = default) where TService : class
        => AppCodeGetNamedServiceHelper.GetService<TService>(owner: this, CodeHelper.Specs, typeName);

    /// <inheritdoc cref="IHasKit{TServiceKit}.Kit"/>
    public ServiceKit16 Kit => field ??= CodeApi.ServiceKit16;

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => CodeHelper.DevTools;

    #endregion

    #region Link & Edit - added to API in 2sxc 10.01

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public ILinkService Link => CodeApi?.Link;

    #endregion


    #region Adam

    /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
    [NonAction]
    public IFile SaveInAdam(NoParamOrder npo = default,
        Stream stream = null,
        string fileName = null,
        string contentType = null,
        Guid? guid = null,
        string field = null,
        string subFolder = "")
        => CtxHlp.SaveInAdam(stream: stream, fileName: fileName, contentType: contentType, guid: guid, field: field, subFolder: subFolder);

    #endregion


    #region New App, Settings, Resources

    /// <inheritdoc />
    public IAppTyped App => CodeApi?.AppTyped;

    /// <inheritdoc cref="ITypedApi.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="ITypedApi.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion

    #region My... Stuff

    private CodeHelperTypedData CodeHelper => field
        ??= new(helperSpecs: new(ExCtxOrNull, false, ((IGetCodePath)this).CreateInstancePath));

    public ITypedItem MyItem => CodeHelper.MyItem;

    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    public ITypedItem MyHeader => CodeHelper.MyHeader;

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
        where T : class, IDataWrapper, new()
        => CodeApi.Cdf.AsStack<T>(items);

    #endregion

    public ITypedRazorModel MyModel => throw new("MyModel isn't meant to work in WebApi"); // v20 CodeHelper.MyModel;

    private CompileCodeHelper CompileCodeHlp => field ??= GetService<CompileCodeHelper>().Init(this);

    /// <inheritdoc cref="ITypedCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder npo = default, string className = default)
        => CompileCodeHlp.CreateInstance(path /*relativePath: (this as IGetCodePath).CreateInstancePath*/, name: className);

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

    #region  CreateInstance implementation

    string IGetCodePath.CreateInstancePath { get; set; }

    #endregion


    #region File Response / Download

    /// <inheritdoc cref="IDynamicWebApi.File"/>
    public dynamic File(NoParamOrder npo = default,
        bool? download = null,
        string virtualPath = null,
        string contentType = null,
        string fileDownloadName = null,
        object contents = null
    ) => new OqtWebApiShim(response: Response, owner: this)
        .File(download: download, virtualPath: virtualPath, contentType: contentType, fileDownloadName: fileDownloadName, contents: contents);

    #endregion

    #region As / AsList WIP v17

    /// <inheritdoc />
    public T As<T>(object source, NoParamOrder npo = default)
        where T : class, IDataWrapper
        => CodeApi.Cdf.AsCustom<T>(source: source);

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder npo = default, bool nullIfNull = default)
        where T : class, IDataWrapper
        => CodeApi.Cdf.AsCustomList<T>(source: source, npo: npo, nullIfNull: nullIfNull);

    #endregion

    #region Customize

    /// <inheritdoc cref="CodeTyped.Customize"/>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    protected ICodeCustomizer Customize => field ??= CodeApi.GetService<ICodeCustomizer>(reuse: true);

    #endregion

}