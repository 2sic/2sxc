using System.Web.Http.Controllers;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

/// <summary>
/// This is the foundation for API controllers using the **Block Context** such as:
/// 
/// 1. the old ToSic.SexyContent.SxcApiController (which is still the default based on the web.config)
/// 2. the old ToSic.Sxc.Dnn.ApiController (was the recommended version for a while)
/// 3. the old Custom.Dnn.Api12
/// 4. the new Custom.Hybrid.Api12, Api14, ApiTyped
/// 5. the current App, DNN, Data, etc.
/// 
/// The class itself has never been published and can be assumed that it is not used anywhere,
/// so rename etc. should never be a problem.
/// </summary>
[PrivateApi("This is an internal base class used for the App ApiControllers. Make sure the implementations don't break")]
// Note: 2022-02 2dm I'm not sure if this was ever published as the official api controller, but it may have been?
[DnnLogExceptions]
// Can't hide in Intellisense, because that would hide it for all derived classes too
// [ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class DnnSxcCustomControllerBase() : DnnApiController, IHasLog // DnnSxcControllerRoot
{
    #region Constructor & DI / Setup

    [PrivateApi]
    protected DnnSxcCustomControllerBase(string logSuffix, string insightsGroup = default): this()
    {
        Log = LogFactory.Create("Api." + logSuffix);
        SysHlp = new(this, insightsGroup ?? EavWebApiConstants.HistoryNameWebApi, null, logApiTimer: true);

    }

    /// <summary>
    /// Special helper to move all Razor logic into a separate class.
    /// For architecture of Composition over Inheritance.
    /// </summary>
    internal DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi, logApiTimer: true);

    /// <inheritdoc />
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public ILog Log => field ??= LogFactory.Create("Api.DynApi"); // if accessed before created, use to-do name


    [PrivateApi]
    protected override void Initialize(HttpControllerContext controllerContext)
    {
        SysHlp.WebApiLogging.OnInitialize(controllerContext);
        base.Initialize(controllerContext);
        var (rootExecutionContext, folder) = CustomCodeHlp.InitExCtxWithBlockSpecOrApp(controllerContext);
        if (this is IGetCodePath thisWithPath)
            thisWithPath.CreateInstancePath = folder;
        ExCtx = rootExecutionContext;
    }

    #endregion

    #region Internal / Plumbing / Obsolete

    internal DnnCustomWebApiHelper CustomCodeHlp => field ??= new DnnCustomWebApiHelper(this, SysHlp).LinkLog(Log);

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public IExecutionContext ExCtx { get; private set; }



    /// <summary>
    /// The name of the logger in insights. The inheriting class should provide the real name to be used.
    /// Note: Probably almost never used, except by 2sic. Must determine if we just remove it
    /// </summary>
    [Obsolete("Deprecated in v13.03 - doesn't serve a purpose any more. Will just remain to avoid breaking public uses of this property.")]
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    protected virtual string HistoryLogGroup => EavWebApiConstants.HistoryNameWebApi;


    protected override void Dispose(bool disposing)
    {
        SysHlp.EndRequestTimer();
        base.Dispose(disposing);
    }

    #endregion

}