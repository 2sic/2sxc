using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlocks
{
    internal sealed class ModuleContentBlock: ContentBlockBase
    {

        public ModuleInfo ModuleInfo;

        public override ContentGroupReferenceManagerBase Manager => new ModuleContentGroupReferenceManager(SxcInstance);

        public override bool ParentIsEntity => false;


        public override ViewDataSource Data => _dataSource 
            ?? (_dataSource = ViewDataSource.ForContentGroupInSxc(SxcInstance, Template, Configuration, Log, ModuleInfo.ModuleID));

        private readonly IEnumerable<KeyValuePair<string, string>> _urlParams;

        /// <summary>
        /// Create a module-content block
        /// </summary>
        /// <param name="moduleInfo">the dnn module-info</param>
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        /// <param name="overrideParams">optional override parameters</param>
        public ModuleContentBlock(ModuleInfo moduleInfo, Log parentLog, IEnumerable<KeyValuePair<string, string>> overrideParams = null): base(parentLog, "CB.Mod")
        {
            ModuleInfo = moduleInfo ?? throw new Exception("Need valid ModuleInfo / ModuleConfiguration of runtime");
            ParentId = moduleInfo.ModuleID;
            ContentBlockId = ParentId;

            // url-params
            _urlParams = overrideParams ?? DnnWebForms.Helpers.SystemWeb.GetUrlParams();
            

            // Ensure we know what portal the stuff is coming from
            // PortalSettings is null, when in search mode
            //PortalSettings = PortalSettings.Current == null || moduleInfo.OwnerPortalID != moduleInfo.PortalID
            //    ? new PortalSettings(moduleInfo.OwnerPortalID)
            //    : PortalSettings.Current;

            Tennant = new DnnTennant(PortalSettings.Current == null || moduleInfo.OwnerPortalID != moduleInfo.PortalID
                ? new PortalSettings(moduleInfo.OwnerPortalID)
                : PortalSettings.Current);


            // important: don't use the SxcInstance.Environment, as it would try to init the Sxc-object before the app is known, causing various side-effects
            ZoneId = new Environment.DnnEnvironment(Log).ZoneMapper.GetZoneId(moduleInfo.OwnerPortalID);// ZoneHelpers.GetZoneId(moduleInfo.OwnerPortalID) ?? 0; // new
            
            AppId = AppHelpers.GetAppIdFromModule(moduleInfo, ZoneId) ?? 0;// fallback/undefined YET

            Log.Add($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            if (AppId == Settings.DataIsMissingInDb)
            {
                _dataIsMissing = true;
                Log.Add("data is missing, will stop here");
                return;
            }

            if (AppId != 0)
            {
                Log.Add("real app, will load data");
                // try to load the app - if possible
                App = new App(ZoneId, AppId, /*PortalSettings*/ Tennant, parentLog: Log);

                Configuration = ConfigurationProvider.GetConfigProviderForModule(moduleInfo.ModuleID, App, SxcInstance);

                // maybe ensure that App.Data is ready
                App.InitData(SxcInstance.Environment.Permissions.UserMayEditContent,
                    SxcInstance.Environment.PagePublishing.IsEnabled(moduleInfo.ModuleID), 
                    Configuration);

                var res = App.ContentGroupManager.GetContentGroupForModule(moduleInfo.ModuleID, moduleInfo.TabID);
                var contentGroupGuid = res.Item1;
                var previewTemplateGuid = res.Item2;
                ContentGroup = App.ContentGroupManager.GetContentGroupOrGeneratePreview(contentGroupGuid, previewTemplateGuid);

                if (ContentGroup.DataIsMissing)
                {
                    _dataIsMissing = true;
                    App = null;
                    return;
                }

                SxcInstance.SetTemplateOrOverrideFromUrl(ContentGroup.Template);                
            }
        }


        public override SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(this, ModuleInfo, _urlParams, parentLog:Log));

        public override bool IsContentApp => ModuleInfo.DesktopModule.ModuleName == "2sxc";

    }
}