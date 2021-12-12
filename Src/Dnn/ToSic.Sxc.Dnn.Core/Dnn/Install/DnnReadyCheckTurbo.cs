using System;
using System.Collections.Concurrent;
using System.IO;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Dnn.Install
{
    /// <summary>
    /// Helper class to ensure that the an app is ready.
    /// It will have to do various file accesses - so once it knows a module is ready, it will cache the result.
    /// </summary>
    public class DnnReadyCheckTurbo: HasLog
    {

        public DnnReadyCheckTurbo(PortalModuleBase module, ILog parentLog) : base("Dnn.PreChk", parentLog)
        {
            _module = module;
        }
        private readonly PortalModuleBase _module;

        /// <summary>
        /// Verify that the portal is ready, otherwise show a good error
        /// </summary>
        public bool EnsureSiteAndAppFoldersAreReady(IBlock block)
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
                Log.Add("Will check if site is ready and template folder exists");
                EnsureSiteIsConfiguredAndTemplateFolderExists(block);

                // If no exception was raised inside, everything is fine - must cache
                CachedModuleResults.AddOrUpdate(_module.ModuleId, true, (id, value) => true);
            }
            else 
                Log.Add("skip, content-block not ready");

            return timerWrap("ok", true);
        }

        /// <summary>
        /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        /// </summary>
        public bool EnsureSiteIsConfiguredAndTemplateFolderExists(IBlock block)
        {
            var wrapLog = Log.SafeCall<bool>($"AppId: {block.AppId}");

            var sexyFolder = new DirectoryInfo(block.Context.Site.AppsRootPhysicalFull);
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, Eav.Constants.ContentAppFolder));
            var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));
            if (!(sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists))
            {
                // configure it
                var tm = block.Context.ServiceProvider.Build<TemplateHelpers>().Init(block.App, block.Log);
                tm.EnsureTemplateFolderExists(false);
            }

            return wrapLog($"Completed init for module {_module.ModuleId} showing {block.AppId}", true);
        }

        internal static ConcurrentDictionary<int, bool> CachedModuleResults = new ConcurrentDictionary<int, bool>();
    }
}