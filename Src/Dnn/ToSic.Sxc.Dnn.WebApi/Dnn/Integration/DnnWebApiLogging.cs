using System.Web;
using System.Web.Http.Controllers;
using ToSic.Eav.Web.Sys;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sys.Code.InfoSystem;

namespace ToSic.Sxc.Dnn.Integration;

/// <summary>
/// Helper class for work on the WebAPIs which may be used by multiple base classes.
/// This is to ensure the code is not in the class, and is reusable. 
/// </summary>
/// <param name="requestLogging">Scoped request logging</param>
/// <param name="firstMessage">First message - ATM only used for queries to simplify/show what query is called.</param>
internal class DnnWebApiLogging(HttpRequestLoggingScoped requestLogging, string firstMessage)
{
    #region Timer Around Everything

    /// <summary>
    /// Add the first message with the current path.
    /// Will not be used until disposed, to then stop the full-timer.
    /// </summary>
    private readonly ILogCall _timerWrapLog = requestLogging.RootLog.Fn(
        // Note: can't log URL as the HttpContext is not ready at this time.
        message: firstMessage ?? $": {HttpContext.Current?.Request.Url.AbsoluteUri}",
        timer: true,
        // ReSharper disable once ExplicitCallerInfoArgument
        cName: "Api"
    );

    internal void OnDispose()
        => _timerWrapLog.Done();

    #endregion


    public void OnInitialize(HttpControllerContext controllerContext)
    {
        controllerContext.Request.Properties.Add(EavLogKey, requestLogging.StoreEntry);
    }

    /// <summary>
    /// Add Log Specs to the current request and also to any reported code changes later on.
    /// </summary>
    public void AddLogSpecs(IBlock block, IApp app, string currentPath, CodeInfosInScope codeInfos) =>
        requestLogging.StoreEntry.TryUpdateSpecs(() =>
        {
            // Add app-id to entry info to ensure controllers with same name being detected by app
            if (currentPath != default && app != default)
                currentPath = $"app:{app.AppId}/{currentPath}";

            var logSpecs = new SpecsForLogHistory()
                .BuildSpecsForLogHistory(block, app, entry: currentPath, addView: false);

            logSpecs["Url"] = HttpContext.Current?.Request.Url.AbsoluteUri;

            codeInfos.AddContext(() => logSpecs, entryPoint: currentPath);

            return logSpecs;
        });
}