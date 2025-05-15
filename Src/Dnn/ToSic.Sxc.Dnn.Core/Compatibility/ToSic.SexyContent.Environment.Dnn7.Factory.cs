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
[Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
public static class Factory
{
    [Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
    public static IBlockBuilder SxcInstanceForModule(int modId, int pageId)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    [Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
    public static IBlockBuilder SxcInstanceForModule(ModuleInfo moduleInfo)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    [Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
    public static IBlockBuilder SxcInstanceForModule(IModule moduleInfo)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());


    [Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
    public static IDynamicCode CodeHelpers(IBlockBuilder blockBuilder)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    /// <summary>
    /// get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <returns></returns>
    [Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
    public static IApp App(int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    /// <summary>
    /// get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <returns></returns>
    [Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
    public static IApp App(int zoneId, int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

    [Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
    public static IApp App(int appId, PortalSettings ownerPortalSettings, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());


    [Obsolete(Sxc.Dnn.Factory.FinallyDecommissionedInV20)]
    public static IApp App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool unusedButKeepForApiStability = false, bool showDrafts = false)
        => throw new NotSupportedException(Sxc.Dnn.Factory.GenerateMessage());

}