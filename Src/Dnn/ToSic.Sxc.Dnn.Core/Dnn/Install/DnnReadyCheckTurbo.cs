using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Concurrent;
using System.IO;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Dnn.Install
{
    /// <summary>
    /// Helper class to ensure that the an app is ready.
    /// It will have to do various file accesses - so once it knows a module is ready, it will cache the result.
    /// </summary>
    public class DnnReadyCheckTurbo : HasLog
    {
        /// <summary>
        /// Fast static check to see if the check had previously completed. 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="block"></param>
        /// <param name="appFolderInitializerLazy"></param>
        /// <param name="log"></param>
        public static void EnsureSiteAndAppFoldersAreReady(PortalModuleBase module, IBlock block, Lazy<AppFolderInitializer> appFolderInitializerLazy, ILog log)
        {
            var wrapLog = log.Call(message: $"Turbo Ready Check: module {module.ModuleId} on page {module.TabId}");
            if (CachedModuleResults.TryGetValue(module.ModuleId, out var exists) && exists)
            {
                // all ok, skip
                wrapLog("quick-check: ready");
                return;
            }

            new DnnReadyCheckTurbo(module, log).EnsureSiteAndAppFoldersAreReadyInternal(block, appFolderInitializerLazy);
            wrapLog("deep-check: ready");
        }

        private DnnReadyCheckTurbo(PortalModuleBase module, ILog parentLog) : base("Dnn.PreChk", parentLog) => _module = module;
        private readonly PortalModuleBase _module;

        /// <summary>
        /// Verify that the portal is ready, otherwise show a good error
        /// </summary>
        private bool EnsureSiteAndAppFoldersAreReadyInternal(IBlock block, Lazy<AppFolderInitializer> appFolderInitializerLazy)
        {
            var timerWrap = Log.Call<bool>(message: $"module {_module.ModuleId} on page {_module.TabId}", useTimer: true);

            if (CachedModuleResults.TryGetValue(_module.ModuleId, out var exists) && exists)
                return timerWrap("Previous check completed, will skip", true);

            // throw better error if SxcInstance isn't available
            // not sure if this doesn't have side-effects...
            if (block?.BlockBuilder == null)
                throw new Exception("Error - can't find 2sxc instance configuration. " +
                                    "Probably trying to show an app or content that has been deleted.");

            // check things if it's a module of this portal (ensure everything is ok, etc.)
            var isSharedModule = _module.ModuleConfiguration.PortalID != _module.ModuleConfiguration.OwnerPortalID;
            if (isSharedModule) return timerWrap("skip, shared", false);

            if (block.App != null)
            {
                Log.A("Will check if site is ready and template folder exists");
                EnsureSiteIsConfiguredAndTemplateFolderExists(block, appFolderInitializerLazy);

                // If no exception was raised inside, everything is fine - must cache
                CachedModuleResults.AddOrUpdate(_module.ModuleId, true, (id, value) => true);
            }
            else
                Log.A("skip, content-block not ready");

            return timerWrap("ok", true);
        }

        /// <summary>
        /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        /// </summary>
        private bool EnsureSiteIsConfiguredAndTemplateFolderExists(IBlock block, Lazy<AppFolderInitializer> appFolderInitializerLazy)
        {
            var wrapLog = Log.Call2<bool>($"AppId: {block.AppId}");

            var sexyFolder = new DirectoryInfo(block.Context.Site.AppsRootPhysicalFull);
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, Eav.Constants.ContentAppFolder));
            var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));
            if (!(sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists))
            {
                // configure it
                var tm = appFolderInitializerLazy.Value.Init(Log);
                tm.EnsureTemplateFolderExists(block.Context.AppState, false);
            }

            return wrapLog.Return(true, $"Completed init for module {_module.ModuleId} showing {block.AppId}");
        }

        internal static ConcurrentDictionary<int, bool> CachedModuleResults = new ConcurrentDictionary<int, bool>();
    }
}