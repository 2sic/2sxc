using System;
using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Run;
using ToSic.Sxc.Web;

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
        /// We don't use a Constructor because of DI
        /// So you must always call Init
        /// </summary>
        public DnnContainer Init(ModuleInfo item, ILog parentLog) 
        {
            Log = new Log("Dnn.Contnr", parentLog);
            Init(item);
            return this;
        }

        /// <summary>
        /// We don't use a Constructor because of DI
        /// So you must always call Init
        /// </summary>
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

        #region Parameters / URL Parameters

        public override List<KeyValuePair<string, string>> Parameters
        {
            get => _parameters ??
                (_parameters = Eav.Factory.Resolve<IHttp>().QueryStringKeyValuePairs());
            set => _parameters = value;
        }
        private List<KeyValuePair<string, string>> _parameters;



#endregion

        /// <inheritdoc />
        public override IBlockIdentifier BlockIdentifier
        {
            get
            {
                if (_blockIdentifier != null) return _blockIdentifier;
                if (UnwrappedContents == null) return null;

                // find ZoneId, AppId and prepare settings for next values
                var zoneId = new DnnZoneMapper().GetZoneId(UnwrappedContents.OwnerPortalID);
                var appId = GetInstanceAppId(zoneId);
                var settings = UnwrappedContents.ModuleSettings;

                // find block identifier
                Guid.TryParse(settings[Settings.FieldContentGroup]?.ToString(), out var blockGuid);

                // Check if we have preview-view identifier - for blocks which don't exist yet
                var previewTemplateString = settings[Settings.FieldPreviewTemplate]?.ToString();
                var overrideView = !string.IsNullOrEmpty(previewTemplateString)
                    ? Guid.Parse(previewTemplateString)
                    : new Guid();

                // Create identifier
                return _blockIdentifier = new BlockIdentifier(zoneId, appId, blockGuid, overrideView);
            }
        }
        private IBlockIdentifier _blockIdentifier;

        private int GetInstanceAppId(int zoneId)
        {
            var wrapLog = Log.Call<int>(parameters: $"..., {zoneId}");

            var module = UnwrappedContents ?? throw new Exception("instance is not ModuleInfo");

            var msg = $"get appid from instance for Z:{zoneId} Mod:{module.ModuleID}";
            var zoneRt = new ZoneRuntime(zoneId, Log);
            if (IsPrimary)
            {
                var appId = zoneRt.DefaultAppId;
                return wrapLog($"{msg} - use Default app: {appId}", appId);
            }

            if (module.ModuleSettings.ContainsKey(Settings.AppNameString))
            {
                var guid = module.ModuleSettings[Settings.AppNameString].ToString();
                var appId = zoneRt.FindAppId(guid);
                return wrapLog($"{msg} AppG:{guid} = app:{appId}", appId);
            }

            Log.Add($"{msg} not found = null");
            return wrapLog("not found", AppConstants.AppIdNotFound);
        }
    }
}
