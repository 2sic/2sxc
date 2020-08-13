using System;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// The DNN implementation of a Block Container (a Module).
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class DnnContainer: Container<ModuleInfo>, IHasLog
    {
        #region Constructors and DI

        /// <summary>
        /// Empty Constructor for DI - must call Init afterwards
        /// </summary>
        public DnnContainer() { }

        public DnnContainer(ModuleInfo item, ILog parentLog) => Init(item, parentLog);

        public DnnContainer Init(ModuleInfo item, ILog parentLog) 
        {
            Log = new Log("Dnn.Contnr", parentLog);
            Init(item);
            return this;
        }

        public override IContainer Init(int moduleId, ILog parentLog) 
        {
            Log = new Log("Dnn.Contnr", parentLog);
            var mod = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, false);
            return Init(mod);
        }

        public ILog Log { get; private set; }
        #endregion


        /// <inheritdoc />
        public override int Id => UnwrappedContents.ModuleID;

        /// <inheritdoc />
        public override int PageId => UnwrappedContents.TabID;

        /// <inheritdoc />
        public override int TenantId => UnwrappedContents.PortalID;

        /// <inheritdoc />
        public override bool IsPrimary => UnwrappedContents.DesktopModule.ModuleName == "2sxc";


        /// <inheritdoc />
        public override IBlockIdentifier BlockIdentifier
        {
            get
            {
                if (_blockIdentifier != null) return _blockIdentifier;
                if (UnwrappedContents == null) return null;

                // find ZoneId
                var zoneId = new DnnZoneMapper().GetZoneId(UnwrappedContents.OwnerPortalID);

                // find AppId
                var appId = GetInstanceAppId(zoneId) ?? AppConstants.AppIdNotFound;

                // find view ID and content-guid
                var settings = UnwrappedContents.ModuleSettings;

                // find block identifier
                Guid.TryParse(settings[Settings.ContentGroupGuidString]?.ToString(), out var blockGuid);

                // Check if we have preview-view identifier - for blocks which don't exist yet
                var previewTemplateString = settings[Settings.PreviewTemplateIdString]?.ToString();
                var overrideView = !string.IsNullOrEmpty(previewTemplateString)
                    ? Guid.Parse(previewTemplateString)
                    : new Guid();

                // Create identifier
                return _blockIdentifier = new BlockIdentifier(zoneId, appId, blockGuid, overrideView);
            }
        }
        private IBlockIdentifier _blockIdentifier;

        private int? GetInstanceAppId(int zoneId)
        {
            var wrapLog = Log.Call<int?>(parameters: $"..., {zoneId}");

            var module = UnwrappedContents ?? throw new Exception("instance is not ModuleInfo");

            var msg = $"get appid from instance for Z:{zoneId} Mod:{module.ModuleID}";
            if (IsPrimary)
            {
                var appId = new ZoneRuntime(zoneId, null).DefaultAppId;
                Log.Add($"{msg} - use def app: {appId}");
                return wrapLog("default", appId);
            }

            if (module.ModuleSettings.ContainsKey(Settings.AppNameString))
            {
                var guid = module.ModuleSettings[Settings.AppNameString].ToString();
                var appId = new ZoneRuntime(zoneId, Log).FindAppId(guid);
                Log.Add($"{msg} AppG:{guid} = app:{appId}");
                return wrapLog("ok", appId);
            }

            Log.Add($"{msg} not found = null");
            return wrapLog("not found", null);
        }
    }
}
