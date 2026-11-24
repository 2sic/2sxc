using ToSic.Eav.Web.Http.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Integration;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

internal class DnnWebApiHelper : ServiceBase
{
    #region Constructor & Destructor

    public DnnWebApiHelper(IHasLog apiController, string historyLogGroup, string firstMessage = default, bool logApiTimer = false) : base("Dnn.ApiHlp")
    {
        var scopedLogging = GetServiceNoExCtx<Generator<HttpRequestLoggingScoped, HttpRequestLoggingScoped.Options>>();
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
    /// Special GetService which knows **nothing** about the context - to explicitly show that this is happening.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <remarks>
    /// This name should be very distinct (NOT GetService())
    /// to make it clear that this is a special case where the context is **not** used.
    ///
    /// In custom code base classes it should use the Execution Context, not this.
    /// </remarks>
    /// <returns></returns>
    public TService GetServiceNoExCtx<TService>() where TService : class
        => _serviceProvider.Get(DnnStaticDi.GetPageScopedServiceProvider).Build<TService>(Log);
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Must cache it, to be really sure we use the same ServiceProvider in the same request
    /// </remarks>
    private readonly LazyGet<ISysServiceProvider> _serviceProvider = new();

    #endregion

    public ICodeLog CodeLog => field ??= new CodeLog(Log);
}
