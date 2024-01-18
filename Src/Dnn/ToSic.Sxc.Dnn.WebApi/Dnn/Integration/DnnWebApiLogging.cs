using System.Web;
using System.Web.Http.Controllers;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.WebApi;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Dnn.Integration;

/// <summary>
/// Helper class for work on the WebAPIs which may be used by multiple base classes.
/// This is to ensure the code is not in the class, and is reusable. 
/// </summary>
internal class DnnWebApiLogging(ILog log, ILogStore logStore, string logGroup, string firstMessage = default)
{
    // Add the first message with the current path
    private readonly ILogCall _timerWrapLog = log.Fn(message: firstMessage ?? $"Path: {HttpContext.Current?.Request.Url.AbsoluteUri}", timer: true);

        // Add it to the log store
    public LogStoreEntry LogStoreEntry = logStore.Add(logGroup ?? EavWebApiConstants.HistoryNameWebApi, log);

    public void OnInitialize(HttpControllerContext controllerContext)
    {
        controllerContext.Request.Properties.Add(DnnConstants.EavLogKey, LogStoreEntry);
    }

    /// <summary>
    /// Add Log Specs to the current request and also to any reported code changes later on.
    /// </summary>
    public void AddLogSpecs(IBlock block, IApp app, string entry, CodeInfosInScope codeInfos)
    {
        try
        {
            if (LogStoreEntry == null) return;

            // Add app-id to entry info to ensure controllers with same name being detected by app
            if (entry != default && app != default)
                entry = "app:" + app.AppId + "/" + entry;

            var logSpecs = new SpecsForLogHistory()
                .BuildSpecsForLogHistory(block, app, entry: entry, addView: false);

            logSpecs["Url"] = HttpContext.Current?.Request.Url.AbsoluteUri;

            LogStoreEntry.UpdateSpecs(logSpecs);
            codeInfos.AddContext(() => logSpecs, entryPoint: entry);
        }
        catch { /* ignore */ }
    }

    public void OnDispose()
    {
        _timerWrapLog.Done();
    }

}