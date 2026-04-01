using DotNetNuke.Entities.Modules;
using System.Web.UI;
using ToSic.Eav.Web.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Render.Sys;

namespace ToSic.Sxc.Dnn;

[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class View : PortalModuleBase, IActionable
{
    private static bool _loggedToBootLog;

    public View()
    {
        if (_loggedToBootLog)
            return;
        _loggedToBootLog = true;
        BootLog.Log.A("Dnn: First moment where View was used.");
    }

    #region GetService and Service Provider

    /// <summary>
    /// Get the service provider only once - ideally in Dnn9.4 we will get it from Dnn
    /// If we would get it multiple times, there are edge cases where it could be different each time! #2614
    /// </summary>
    private IServiceProvider ServiceProvider => Runtime.ServiceProvider;

    private TService GetService<TService>() => Runtime.GetService<TService>(Log);

    #endregion

    /// <summary>
    /// Block needs to self-initialize when first requested, because it's used in the Actions-Menu builder
    /// which runs before page-load
    /// </summary>
    private IBlock Block => Runtime.Block;

    #region Logging

    /// <summary>
    /// Get the request logging helper for this request.
    /// </summary>
    /// <remarks>
    /// - Must use ServiceProvider.Build directly, as the `GetService` method would try to use the Log before it's created.
    /// - delay creating until first use to really just track our time when needed.
    /// </remarks>
    private HttpRequestLoggingScoped RequestLogging => Runtime.RequestLogging;

    private ILog Log => Runtime.Log;

    /// <summary>
    /// Log Timer to use everywhere we want to track the cumulative time.
    /// </summary>
    /// <remarks>
    /// Time must be false, as it will be started/stopped as needed within the DoInTimer methods.
    /// </remarks>
    protected ILogCall LogTimer => field ??= Log.Fn(timer: false);

    #endregion

    /// <summary>
    /// Page Load event
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        Runtime.BeginRequest(ModuleConfiguration.ModuleTitle);

        LogTimer.DoInTimer(() =>
        {
            var l = Log.Fn(message: nameof(Page_Load), timer: true);
            TryCatchAndLogToDnn(() =>
            {
                Runtime.Prepare(this);
                _dnnClientResources = GetService<DnnClientResources>().Init(Page);
                return true;
            });
            l.Done();
        });
    }

    private DnnClientResources _dnnClientResources;

    /// <summary>
    /// Process View if a Template has been set
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        var l = Log.Fn();
        var finalMessage = "";
        LogTimer.DoInTimer(() =>
        {
            IRenderResult renderResult = null;
            var headersAndScriptsAdded = false;

            if (!IsError)
                TryCatchAndLogToDnn(() =>
                {
                    var renderState = Runtime.Render(RenderNaked);
                    renderResult = renderState.RenderResult;
                    finalMessage = renderState.FinalMessage;

                    try
                    {
                        GetService<DnnPageChanges>().Apply(Page, renderResult);
                    }
                    catch
                    {
                        /* ignore */
                    }

                    _dnnClientResources.AddEverything(renderResult.Features);
                    headersAndScriptsAdded = true;

                    if (RenderNaked)
                        SendStandalone(renderResult.Html);
                    else
                        phOutput.Controls.Add(new LiteralControl(renderResult.Html));

                    return true;
                });

            if (IsError && !headersAndScriptsAdded)
                _dnnClientResources?.AddEverything(renderResult?.Features);
        });

        LogTimer.Timer.Start();
        LogTimer.Done(IsError ? "⚠️" : finalMessage);
        l.Done();
    }

    private ModuleViewRuntime Runtime => field ??= new(ModuleConfiguration, ModuleId, TabId, DnnStaticDi.CreateModuleScopedServiceProvider(), GetOptionalDetailedLogToAttach);
}
