using DotNetNuke.Entities.Modules;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Statics;

namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// This object is for other engines like razor or WebForms wanting to work with 2sxc data. 
    /// Just initialize the object with tab-id and module id, 
    /// you then get
    /// - SxcInstance - the core instance object containing App, Template, etc.
    /// - App
    /// </summary>
    public class EngineContext
    {
        public int TabId { get; }
        public int ModuleId { get; }

        public EngineContext(int tabId, int modId)
        {
            TabId = tabId;
            ModuleId = modId;
        }

        private ModuleInfo _moduleInfo;
        public ModuleInfo ModuleInfo => _moduleInfo ??
                                        (_moduleInfo = ModuleController.Instance.GetModule(ModuleId,
                                            TabId, false));

        private InstanceContext _sxcInstance;

        public InstanceContext SxcInstance
        {
            get
            {
                if(_sxcInstance!= null)
                    return _sxcInstance;
                int? appId;
                appId = AppHelpers.GetAppIdFromModule(ModuleInfo);
                var zoneId = ZoneHelpers.GetZoneID(ModuleInfo.PortalID);
                _sxcInstance = new InstanceContext(zoneId.Value, appId.Value, true, ModuleInfo.OwnerPortalID, ModuleInfo);
                    return _sxcInstance;
            }
        }

        private AppAndDataHelpers _appAndDataHelpers;
        public AppAndDataHelpers Helpers()
        {
            if (_appAndDataHelpers != null)
                return _appAndDataHelpers;

            var viewDataSource = SxcInstance.DataSource;// ViewDataSource.ForModule(ModuleInfo.ModuleID, SecurityHelpers.HasEditPermission(ModuleInfo), SxcInstance.ContentGroups.GetContentGroupForModule(ModuleInfo.ModuleID).Template, SxcInstance);
            _appAndDataHelpers = new AppAndDataHelpers(SxcInstance, ModuleInfo, viewDataSource, SxcInstance.App);

            return _appAndDataHelpers;
        }
    }
}