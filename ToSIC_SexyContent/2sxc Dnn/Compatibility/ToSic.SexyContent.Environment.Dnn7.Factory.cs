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
        //{
        //    var moduleInfo = new ModuleController().GetModule(modId, tabId, false);
        //    var instance = new DnnInstanceInfo(moduleInfo);
        //    return SxcInstanceForModule(instance);
        //}

        [Obsolete("use ToSic.Sxc.Dnn.Factory.CmsBlock(...) instead")]
        public static IBlockBuilder SxcInstanceForModule(ModuleInfo moduleInfo)
            => Sxc.Dnn.Factory.CmsBlock(moduleInfo);
            //=> SxcInstanceForModule(new DnnInstanceInfo(moduleInfo));

        [Obsolete("use ToSic.Sxc.Dnn.Factory.CmsBlock(...) instead")]
        public static IBlockBuilder SxcInstanceForModule(IContainer moduleInfo)
            => Sxc.Dnn.Factory.CmsBlock(moduleInfo);
        //{
        //    var dnnModule = ((Container<ModuleInfo>) moduleInfo).Original;
        //    var tenant = new DnnTenant(new PortalSettings(dnnModule.OwnerPortalID));
        //    return new BlockFromModule(moduleInfo, parentLog: null, tenant: tenant).CmsInstance;
        //}

        [Obsolete("use ToSic.Sxc.Dnn.Factory.DynamicCode(...) instead")]
        public static IDynamicCode CodeHelpers(IBlockBuilder cms) 
            => Sxc.Dnn.Factory.DynamicCode(cms);
            //=> new DynamicCodeHelper(cms as CmsBlock);

        /// <summary>
        /// get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <returns></returns>
        [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
        public static IApp App(int appId, bool versioningEnabled = false, bool showDrafts = false)
            => Sxc.Dnn.Factory.App(appId, versioningEnabled, showDrafts);
        //=> App(appId, PortalSettings.Current, versioningEnabled, showDrafts);

        /// <summary>
        /// get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <returns></returns>
        [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
        public static IApp App(int zoneId, int appId, bool versioningEnabled = false, bool showDrafts = false) 
            => Sxc.Dnn.Factory.App(appId, versioningEnabled, showDrafts);
            //=> App(zoneId, appId, PortalSettings.Current, versioningEnabled, showDrafts);

        [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
        public static IApp App(int appId, PortalSettings ownerPortalSettings, bool versioningEnabled = false, bool showDrafts = false)
            => Sxc.Dnn.Factory.App(appId, ownerPortalSettings, versioningEnabled, showDrafts);
        //{
        //    // 2018-09-22 new
        //    var appStuff = new App(new DnnTenant(ownerPortalSettings), Eav.Apps.App.AutoLookupZone, appId, 
        //        ConfigurationProvider.Build(showDrafts, versioningEnabled, new TokenListFiller()), true, null);
        //    return appStuff;
        //}

        [Obsolete("use ToSic.Sxc.Dnn.Factory.App(...) instead")]
        public static IApp App(int zoneId, int appId, PortalSettings ownerPortalSettings, bool versioningEnabled = false, bool showDrafts = false)
            => Sxc.Dnn.Factory.App(appId, ownerPortalSettings, versioningEnabled, showDrafts);
        //{
        //    var appStuff = new App(new DnnTenant(ownerPortalSettings), zoneId, appId,
        //        ConfigurationProvider.Build(showDrafts, versioningEnabled, new TokenListFiller()), true, null);
        //    return appStuff;
        //}

    }
}