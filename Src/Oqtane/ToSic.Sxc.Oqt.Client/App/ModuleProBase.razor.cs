using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Oqt.Client;
using ToSic.Sxc.Oqt.Shared.Helpers;
using ToSic.Sxc.Oqt.Shared.Interfaces;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class ModuleProBase: ModuleBase, IOqtHybridLog
{
    #region Injected Services

    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IOqtDebugStateService OqtDebugStateService { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }

    #endregion

    #region Shared Variables

    public bool IsSuperUser => _isSuperUser.Get(() => UserSecurity.IsAuthorized(PageState.User, RoleNames.Host));
    private readonly GetOnce<bool> _isSuperUser = new();

    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public bool IsAdmin => _isAdmin.Get(() => UserSecurity.IsAuthorized(PageState.User, RoleNames.Admin));
    private readonly GetOnce<bool> _isAdmin = new();

    public SxcInterop SxcInterop;
    public bool IsSafeToRunJs;
    public readonly ConcurrentQueue<object[]> LogMessageQueue = new();

    public bool FirstRender = true;

    #endregion

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var debugEnabled = await OqtDebugStateService.GetDebugAsync();
        if (!debugEnabled && NavigationManager.TryGetQueryString<bool>("debug", out var debugInQueryString))
            OqtDebugStateService.SetDebug(debugInQueryString);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            SxcInterop = new(JSRuntime);
            // now we are safe to have SxcInterop and run js
            IsSafeToRunJs = true;
        }
        FirstRender = firstRender;
    }

    /// <summary>
    /// Determines if the current page is in the prerendering phase.
    /// </summary>
    /// <remarks>
    /// This is a 2sxc implementation which provides an approximation suitable for PreRendering purposes. 
    /// However, it may not always return accurate results, especially with "WebAssemblyPrerendered" where it might return an incorrect true value.
    /// This behavior is acceptable for our PreRendering support.
    /// It's important to note that we cannot solely rely on Oqtane.Shared.SiteState.IsPrerendering property. 
    /// In Oqtane, this property indicates that a page isn't prerendering if the response has already started, which differs from our use-case.
    /// </remarks>
    /// <returns>True if the page is in the prerendering phase; otherwise, false.</returns>
    public bool IsPrerendering() =>
        (PageState.Site.Prerender) // Checks the site's render mode.
        && FirstRender // Ensures this is the first render.
        && SiteState.IsPrerendering; // Validates the prerendering state of the current page.

    #region Log Helpers
    /// <summary>
    /// console.log
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public void Log(params object[] message)
    {
        // If the url has a debug=true and we are the super-user
        if (message == null || !message.Any() || !OqtDebugStateService.IsDebugEnabled || !IsSuperUser) return;

        _logPrefix ??= $"2sxc:Page({PageState?.Page?.PageId}):Mod({ModuleState?.ModuleId}):Render({ModuleState?.RenderId}):";
        try
        {
            // log on web server / webassembly console
            foreach (var item in message)
                Console.WriteLine($"{_logPrefix} {item}");

            if (OqtDebugStateService.Platform == "Server")
            {
                // log to browser console
                if (IsSafeToRunJs)
                {
                    // first log messages from queue
                    var timeOut = 0;
                    while (!LogMessageQueue.IsEmpty && timeOut < 100)
                    {
                        if (LogMessageQueue.TryDequeue(out var messageToLog))
                        {
                            ConsoleLog(new List<object> { $"dequeue({LogMessageQueue.Count}):" }.Concat(messageToLog).ToArray());
                            timeOut = 0;
                        }
                        else
                            timeOut++;
                    };

                    // than log current message
                    ConsoleLog(message);
                }
                else
                {
                    // browser is not ready, so store messages in queue
                    LogMessageQueue.Enqueue(message.ToArray());
                }
            }

            // log to oqtane log if possible
            try
            {
                foreach (var item in message)
                    logger.LogDebug($"{_logPrefix} {item}");
            }
            catch
            {
                // sink
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }
        
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public void LogError(Exception ex) => LogError(ErrorHelper.ErrorMessage(ex, IsSuperUser || IsAdmin));
        
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public void LogError(string errorMessage)
    {
        try
        {
            // log to web server log
            Console.WriteLine($"Error:{_logPrefix}:{errorMessage}");

            // log to browser console
            if (IsSafeToRunJs)
                JSRuntime.InvokeVoidAsync(ConsoleLogJs, "Error:", _logPrefix, errorMessage);
            else
                LogMessageQueue.Enqueue(new List<object> { "Error:", _logPrefix, errorMessage }.ToArray());

            // log error to oqtane log if possible
            try
            {
                logger.LogError(errorMessage);
            }
            catch
            {
                // sink
            }
            AddModuleMessage(errorMessage, MessageType.Warning);
        }
        catch
        {
            // sink
        }
    }
        
    private void ConsoleLog(object[] message)
    {
        var data = new List<object> { _logPrefix }.Concat(message);
        JSRuntime.InvokeVoidAsync(ConsoleLogJs, data.ToArray());
    } 
    private string _logPrefix;
    private const string ConsoleLogJs = "console.log";
    #endregion

    public bool IsDev => Configuration["Environment"] == "Development";
}