using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Client;
using Interop = ToSic.Sxc.Oqt.Client.Interop;

// TODO: @STV
// We should move the logging / user-check etc. into a base class for SoC
// I started this but didn't finish it, because
// I can see the previous code has some random placements of calling the base-events.
// So they are often not at the beginning, which is what I would have expected
// and would would be necessary for a base-class to do its work. 
// Pls check and probably fix (I think the base call should be on top in 99% of all cases)
// And then inherit from this and slim down the code in the main index

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public class ModuleProBase: ModuleBase
    {
        #region Injected Services

        [Inject] public NavigationManager NavigationManager { get; set; }

        #endregion

        #region Shared Variables

        public bool Debug;
        public bool IsSuperUser => _isSuperUser ??= UserSecurity.IsAuthorized(PageState.User, RoleNames.Host);
        private bool? _isSuperUser;
        private bool _isSafeToRunJs;

        /// <summary>
        /// JS Interop, will be initialized in OnInitializedAsync
        /// </summary>
        public Interop Interop;

        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Interop ??= new Interop(JSRuntime);
        }


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            NavigationManager.TryGetQueryString("debug", out Debug);
            await Log($"2sxc Blazor Logging Enabled");  // will only show if it's enabled
        }
        

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender) _isSafeToRunJs = true; // now we are safe to have Interop and run js
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
                Console.WriteLine($"{_logPrefix} {message.FirstOrDefault()}");
                // log to browser console
                if (_isSafeToRunJs)
                {
                    var data = new List<object> { _logPrefix }.Concat(message);
                    await JSRuntime.InvokeVoidAsync(ConsoleLogJs, data.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{_logPrefix}:{ex.Message}");
                if (_isSafeToRunJs)
                    await JSRuntime.InvokeVoidAsync(ConsoleLogJs, "Error:", _logPrefix, ex.Message);
            }
        }
        private string _logPrefix;
        private const string ConsoleLogJs = "console.log";

        #endregion
    }
}
