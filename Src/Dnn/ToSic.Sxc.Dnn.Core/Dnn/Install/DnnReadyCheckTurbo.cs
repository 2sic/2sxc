using DotNetNuke.Entities.Modules;
using System.Collections.Concurrent;
using System.IO;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Dnn.Install;

/// <summary>
/// Helper class to ensure that an app is ready.
/// It will have to do various file accesses - so once it knows a module is ready, it will cache the result.
/// </summary>
internal class DnnReadyCheckTurbo(LazySvc<AppFolderInitializer> appFolderInitializerLazy)
    : ServiceBase("Dnn.PreChk", connect: [appFolderInitializerLazy])
{
    /// <summary>
    /// Fast static check to see if the check had previously completed. 
    /// </summary>
    /// <param name="module"></param>
    /// <param name="log"></param>
    public static bool QuickCheckSiteAndAppFoldersAreReady(PortalModuleBase module, ILog log)
    {
        var l = log.Fn<bool>($"module: {module.ModuleId}; page: {module.TabId}");
        return CachedModuleResults.TryGetValue(module.ModuleId, out var exists) && exists
            ? l.ReturnTrue("quick-check: ready")
            : l.ReturnFalse("deep-check: not ready, must do extensive check");
    }

    /// <summary>
    /// Verify that the portal is ready, otherwise show a good error
    /// </summary>
    public bool EnsureSiteAndAppFoldersAreReady(PortalModuleBase module, IBlock block)
    {
        var l = Log.Fn<bool>(timer: true, message: $"module {module.ModuleId} on page {module.TabId}");
        if (CachedModuleResults.TryGetValue(module.ModuleId, out var exists) && exists)
            return l.ReturnTrue("Previous check completed, will skip");

        // throw better error if SxcInstance isn't available
        // not sure if this doesn't have side-effects...
        if (block?.BlockBuilder == null)
            throw l.Done(new Exception("Error - can't find 2sxc instance configuration. " +
                                       "Probably trying to show an app or content that has been deleted or not yet installed. " +
                                       "You may also have EnterpriseCMS features enabled but are missing the license activation (but this is super rare). "));

        // check things if it's a module of this portal (ensure everything is ok, etc.)
        var isSharedModule = module.ModuleConfiguration.PortalID != module.ModuleConfiguration.OwnerPortalID;
        if (isSharedModule) return l.ReturnFalse("skip, shared");

        if (block.App != null)
        {
            l.A("Will check if site is ready and template folder exists");
            EnsureSiteIsConfiguredAndTemplateFolderExists(module, block);

            // If no exception was raised inside, everything is fine - must cache
            CachedModuleResults.AddOrUpdate(module.ModuleId, true, (id, value) => true);
        }
        else
            l.A("skip, content-block not ready");

        return l.ReturnTrue("ok");
    }

    /// <summary>
    /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
    /// </summary>
    private void EnsureSiteIsConfiguredAndTemplateFolderExists(PortalModuleBase module, IBlock block)
    {
        var l = Log.Fn($"module {module.ModuleId} on page {module.TabId}");
        var sxcFolder = new DirectoryInfo(block.Context.Site.AppsRootPhysicalFull);
        var contentFolder = new DirectoryInfo(Path.Combine(sxcFolder.FullName, Eav.Constants.ContentAppFolder));
        var webConfigTemplate = new FileInfo(Path.Combine(sxcFolder.FullName, SpecialFiles.WebConfigFileName));
        if (!(sxcFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists))
        {
            // configure it
            var tm = appFolderInitializerLazy.Value;
            tm.EnsureTemplateFolderExists(block.Context.AppReader.Specs.Folder, false);
        }

        l.Done($"Completed init App {block.AppId}");
    }

    internal static ConcurrentDictionary<int, bool> CachedModuleResults = new();
}