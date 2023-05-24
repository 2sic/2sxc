using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Client;
using ToSic.Sxc.Oqt.Shared.Interfaces;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
  public class ModuleProBase: ModuleBase, IOqtHybridLog
  {
        #region Injected Services

        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IOqtDebugStateService OqtDebugStateService { get; set; }

        #endregion

        #region Shared Variables

        public bool IsSuperUser => _isSuperUser ??= UserSecurity.IsAuthorized(PageState.User, RoleNames.Host);
        private bool? _isSuperUser;

        public SxcInterop SxcInterop;
        public bool IsSafeToRunJs;
        public readonly ConcurrentQueue<object[]> LogMessageQueue = new();

        #endregion

        //protected override async Task OnInitializedAsync()
        //{
        //    await base.OnInitializedAsync();
        //}
        public bool IsPreRendering() => 
            (PageState.Site.RenderMode is "ServerPrerendered" or "WebAssemblyPrerendered") // The render mode for the site.
            || PageState.QueryString.ContainsKey("prerender"); // used for testing, just add to page url in query string ("?prerender")

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var debugEnabled = await OqtDebugStateService.GetDebugAsync();
            if (!debugEnabled && NavigationManager.TryGetQueryString<bool>("debug", out var debugInQueryString))
                OqtDebugStateService.SetDebug(debugInQueryString);

            Log($"2sxc Blazor Logging Enabled");  // will only show if it's enabled
        }
        

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                SxcInterop = new SxcInterop(JSRuntime);
                // now we are safe to have SxcInterop and run js
                IsSafeToRunJs = true;
            } 
        }

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

            _logPrefix ??= $"2sxc:Page({PageState?.Page?.PageId}):Mod({ModuleState?.ModuleId}):";
            try
            {
                // log on web server
                foreach (var item in message)
                    Console.WriteLine($"{_logPrefix} {item}");

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
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{_logPrefix}:{ex.Message}");
                if (IsSafeToRunJs)
                    JSRuntime.InvokeVoidAsync(ConsoleLogJs, "Error:", _logPrefix, ex.Message);
                else
                    LogMessageQueue.Enqueue(new List<object> { "Error:", _logPrefix, ex.Message }.ToArray());
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
    }
}
