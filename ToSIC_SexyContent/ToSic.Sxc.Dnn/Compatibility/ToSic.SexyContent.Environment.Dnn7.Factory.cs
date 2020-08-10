using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web;
using IApp = ToSic.Sxc.Apps.IApp;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// This is a factory to create 2sxc-instance objects and related objects from
    /// non-2sxc environments.
    /// </summary>
    [Obsolete("use ToSic.Sxc.Dnn.Factory instead")]
    public static class Factory
    {
        [Obsolete("use ToSic.Sxc.Dnn.Factory.CmsBlock(tabId, modId) instead. Note that tab/mod are reversed to this call.")]
        public static IBlockBuilder SxcInstanceForModule(int modId, int tabId)
            => Sxc.Dnn.Factory.CmsBlock(tabId, modId);

        [Obsolete("use ToSic.Sxc.Dnn.Factory.CmsBlock(...) instead")]
        public static IBlockBuilder SxcInstanceForModule(ModuleInfo moduleInfo)
            => Sxc.Dnn.Factory.CmsBlock(moduleInfo);

        [Obsolete("use ToSic.Sxc.Dnn.Factory.CmsBlock(...) instead")]
        public static IBlockBuilder SxcInstanceForModule(IContainer moduleInfo)
            => Sxc.Dnn.Factory.CmsBlock(moduleInfo);


        [Obsolete("use ToSic.Sxc.Dnn.Factory.DynamicCode(...) instead")]
        public static IDynamicCode CodeHelpers(IBlockBuilder blockBuilder) 
            => Sxc.Dnn.Factory.DynamicCode(blockBuilder);

        /// <summary>
        /// get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <returns></returns>
        [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
        public static IApp App(int appId, bool versioningEnabled = false, bool showDrafts = false)
            => Sxc.Dnn.Factory.App(appId, versioningEnabled, showDrafts);

        /// <summary>
        /// get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <returns></returns>
        [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
        public static IApp App(int zoneId, int appId, bool versioningEnabled = false, bool showDrafts = false) 
            => Sxc.Dnn.Factory.App(appId, versioningEnabled, showDrafts);

        [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
        public static IApp App(int appId, PortalSettings ownerPortalSettings, bool versioningEnabled = false, bool showDrafts = false)
            => Sxc.Dnn.Factory.App(appId, ownerPortalSettings, versioningEnabled, showDrafts);


        [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
        public static IApp App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool versioningEnabled = false, bool showDrafts = false)
            => Sxc.Dnn.Factory.App(appId, ownerPortalSettings, versioningEnabled, showDrafts);

    }
}