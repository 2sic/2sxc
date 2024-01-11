using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using IApp = ToSic.Sxc.Apps.IApp;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7;

/// <summary>
/// This is a factory to create 2sxc-instance objects and related objects from
/// non-2sxc environments.
/// </summary>
[Obsolete("use ToSic.Sxc.Dnn.Factory instead")]
public static class Factory
{
    [Obsolete("use ToSic.Sxc.Dnn.Factory.CmsBlock(tabId, modId) instead. Note that tab/mod are reversed to this call.")]
    public static IBlockBuilder SxcInstanceForModule(int modId, int pageId)
        => Sxc.Dnn.Factory.CmsBlock(pageId, modId);

    [Obsolete("use ToSic.Sxc.Dnn.Factory.CmsBlock(...) instead")]
    public static IBlockBuilder SxcInstanceForModule(ModuleInfo moduleInfo)
        => Sxc.Dnn.Factory.CmsBlock(moduleInfo);

    [Obsolete("use ToSic.Sxc.Dnn.Factory.CmsBlock(...) instead")]
    public static IBlockBuilder SxcInstanceForModule(IModule moduleInfo)
        => Sxc.Dnn.Factory.CmsBlock(moduleInfo);


    [Obsolete("use ToSic.Sxc.Dnn.Factory.DynamicCode(...) instead")]
    public static IDynamicCode CodeHelpers(IBlockBuilder blockBuilder) 
        => Sxc.Dnn.Factory.DynamicCode(blockBuilder);

    /// <summary>
    /// get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <returns></returns>
    [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
    public static IApp App(int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => Sxc.Dnn.Factory.App(appId, unusedButKeepForApiStability, showDrafts);

    /// <summary>
    /// get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <returns></returns>
    [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
    public static IApp App(int zoneId, int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false) 
        => Sxc.Dnn.Factory.App(appId, unusedButKeepForApiStability, showDrafts);

    [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
    public static IApp App(int appId, PortalSettings ownerPortalSettings, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => Sxc.Dnn.Factory.App(appId, ownerPortalSettings, unusedButKeepForApiStability, showDrafts);


    [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
    public static IApp App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => Sxc.Dnn.Factory.App(appId, ownerPortalSettings, unusedButKeepForApiStability, showDrafts);

}