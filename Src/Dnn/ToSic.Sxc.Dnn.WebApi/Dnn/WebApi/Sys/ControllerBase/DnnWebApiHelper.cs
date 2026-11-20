using ToSic.Eav.Web.Http.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

internal class DnnWebApiHelper : ServiceWithContext
{
    #region Constructor & Destructor

    public DnnWebApiHelper(IHasLog apiController, string historyLogGroup, string firstMessage = default, bool logApiTimer = false) : base("Dnn.ApiHlp")
    {
        var scopedLogging = GetService<Generator<HttpRequestLoggingScoped, HttpRequestLoggingScoped.Options>>();
        var requestLogging = scopedLogging.New(new() { Segment = historyLogGroup ?? EavWebApiConstants.HistoryNameWebApi, RootName = "Dnn.Api" });
        this.LinkLog(requestLogging.RootLog);
        apiController.LinkLog(requestLogging.RootLog);
        if (logApiTimer)
            WebApiLogging = new(requestLogging, firstMessage: firstMessage);
    }

    #endregion

    #region Logging

    internal DnnWebApiLogging WebApiLogging { get; }

    public void EndRequestTimer() => WebApiLogging?.OnDispose();

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
    private readonly LazyGet<IServiceProvider> _serviceProvider = new();

    #endregion

    public IBlock? GetBlockAndContext(HttpRequestMessage request) 
        => _blcCtx.Get(() => GetService<DnnGetBlock>().TryGetBlockSpecsUsingModuleAndContentBlockHeaders(request));
    private readonly LazyGet<IBlock?> _blcCtx = new();


    public ICodeLog CodeLog => field ??= new CodeLog(Log);
}