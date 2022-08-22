using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
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
using Interop = ToSic.Sxc.Oqt.Client.Interop;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public class ModuleProBase: ModuleBase
    {
        #region Injected Services

        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IHttpContextAccessor HttpContextAccessor { get; set; }

        #endregion

        #region Shared Variables

        //public static bool Debug;

        public bool Debug // persist state across circuits (blazor server only)
        {
            get => (HttpContextAccessor?.HttpContext?.Items[DebugKey] as bool?) ?? false;
            set
            {
                if (HttpContextAccessor?.HttpContext != null)
                    HttpContextAccessor.HttpContext.Items[DebugKey] = value;
            }
        }
        private const string DebugKey = "Debug";

        public bool IsSuperUser => _isSuperUser ??= UserSecurity.IsAuthorized(PageState.User, RoleNames.Host);
        private bool? _isSuperUser;

        public bool IsSafeToRunJs;

        #endregion

        //protected override async Task OnInitializedAsync()
        //{
        //    await base.OnInitializedAsync();
        //}


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (NavigationManager.TryGetQueryString<bool>("debug", out var debugInQueryString))
                Debug = debugInQueryString;
            
            await Log($"2sxc Blazor Logging Enabled");  // will only show if it's enabled
        }
        

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender) IsSafeToRunJs = true; // now we are safe to have Interop and run js
        }

        #region Log Helpers

        /// <summary>
        /// console.log
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Log(params object[] message)
        {
            // If the url has a debug=true and we are the super-user
            if (message == null || !message.Any() || !Debug || !IsSuperUser) return;

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
                    while (!_logMessageQueue.IsEmpty && timeOut < 100)
                    {
                        if (_logMessageQueue.TryDequeue(out var messageToLog))
                        {
                            await ConsoleLog(new List<object> { $"dequeue({_logMessageQueue.Count}):" }.Concat(messageToLog).ToArray());
                            timeOut = 0;
                        }
                        else
                            timeOut++;
                    };
                    
                    // than log current message
                    await ConsoleLog(message);
                }
                else
                {
                    // browser is not ready, so store messages in queue
                    _logMessageQueue.Enqueue(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{_logPrefix}:{ex.Message}");
                if (IsSafeToRunJs)
                    await JSRuntime.InvokeVoidAsync(ConsoleLogJs, "Error:", _logPrefix, ex.Message);
                else
                    _logMessageQueue.Enqueue(new List<object> { "Error:", _logPrefix, ex.Message }.ToArray());
            }
        }

        private async Task ConsoleLog(object[] message)
        {
            var data = new List<object> { _logPrefix }.Concat(message);
            await JSRuntime.InvokeVoidAsync(ConsoleLogJs, data.ToArray());
        }
        private string _logPrefix;
        private const string ConsoleLogJs = "console.log";
        private readonly ConcurrentQueue<object[]> _logMessageQueue = new();

        #endregion
    }
}
