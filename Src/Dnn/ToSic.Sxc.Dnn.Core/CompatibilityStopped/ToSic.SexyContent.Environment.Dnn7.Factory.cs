using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Render.Block.Sys;
using IApp = ToSic.Sxc.Apps.IApp;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7;

/// <summary>
/// Deprecated since v13, announced for removal in v15, removed in v20.
/// </summary>
[Obsolete]
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class Factory
{
    [Obsolete]
    public static IBlockRenderer SxcInstanceForModule(int modId, int pageId)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    [Obsolete]
    public static IBlockRenderer SxcInstanceForModule(ModuleInfo moduleInfo)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    [Obsolete]
    public static IBlockRenderer SxcInstanceForModule(IModule moduleInfo)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());


    [Obsolete]
    public static IDynamicCode CodeHelpers(IBlockRenderer blockRenderer)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    /// <summary>
    /// get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <returns></returns>
    [Obsolete]
    public static IApp App(int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    /// <summary>
    /// get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <returns></returns>
    [Obsolete]
    public static IApp App(int zoneId, int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    [Obsolete]
    public static IApp App(int appId, PortalSettings ownerPortalSettings, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());


    [Obsolete]
    public static IApp App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

}