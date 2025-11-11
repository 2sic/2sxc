using System.Web;
using ToSic.Eav.Web.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Dnn.Integration;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

internal class DnnWebApiHelper : CodeHelperBase
{
    #region Constructor & Destructor

    public DnnWebApiHelper(IHasLog apiController, string historyLogGroup, string firstMessage = default) : base("Dnn.ApiHlp")
    {
        var requestLogging = GetService<Generator<HttpRequestLoggingScoped, HttpRequestLoggingScoped.Opts>>()
                .New(new() { Segment = historyLogGroup ?? EavWebApiConstants.HistoryNameWebApi, RootName = "Dnn.Api" });
        this.LinkLog(requestLogging.RootLog);
        apiController.LinkLog(requestLogging.RootLog);
        WebApiLogging = new(requestLogging, firstMessage: firstMessage);
    }

    public void OnDispose()
        => WebApiLogging.OnDispose();

    #endregion

    #region Logging

    public DnnWebApiLogging WebApiLogging { get; }

    #endregion

    #region Basic Service

    /// <summary>
    /// Get a service of a specified type. 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// This will override the base functionality to ensure that any services created will be able to get the CodeContext.
    /// </remarks>
    public TService GetService<TService>() where TService : class
        => ExCtxOrNull?.GetService<TService>()
            ?? _serviceProvider.Get(DnnStaticDi.GetPageScopedServiceProvider).Build<TService>(Log);
    // Must cache it, to be really sure we use the same ServiceProvider in the same request
    private readonly GetOnce<IServiceProvider> _serviceProvider = new();

    public void SetupResponseMaker(System.Web.Http.ApiController apiController)
        => GetService<IResponseMaker>().Init(apiController);

    #endregion

    /// <summary>
    ///  Extend Time so Web Server doesn't time out
    /// </summary>
    public void PreventServerTimeout600()
        => HttpContext.Current.Server.ScriptTimeout = 600;

    public IBlock GetBlockAndContext(HttpRequestMessage request) 
        => _blcCtx.Get(() => GetService<DnnGetBlock>().GetCmsBlock(request));
    private readonly GetOnce<IBlock> _blcCtx = new();


}