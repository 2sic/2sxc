using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.WebApi.Validation;


// Todo: MVC - has a DNN folder name in this code
// must be injected from elsewhere

namespace ToSic.Sxc.WebApi.Features
{
    public class FeaturesBackend: WebApiBackendBase<FeaturesBackend>
    {
        #region Constructor / DI

        public FeaturesBackend(IZoneMapper zoneMapper, IServiceProvider serviceProvider, 
            IGlobalConfiguration globalConfiguration, IFeaturesInternal features, SystemLoader systemLoader) : base(serviceProvider, "Bck.Feats")
        {
            _zoneMapper = zoneMapper;
            _globalConfiguration = globalConfiguration;
            _features = features;
            _systemLoader = systemLoader;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly IFeaturesInternal _features;
        private readonly SystemLoader _systemLoader;

        public new FeaturesBackend Init(ILog parentLog)
        {
            base.Init(parentLog);
            _zoneMapper.Init(Log);
            return this;
        }

        #endregion

        public IEnumerable<Feature> GetAll(bool reload)
        {
            if (reload) _systemLoader.Reload();
            return _features.All;
        }


        public bool SaveFeatures(FeaturesDto featuresManagementResponse)
        {
            // first do a validity check 
            if (featuresManagementResponse?.Msg?.Features == null) return false;

            // 1. valid json? 
            // - ensure signature is valid
            if (!Json.IsValidJson(featuresManagementResponse.Msg.Features)) return false;

            // then take the newFeatures (it should be a json)
            // and save to /desktopmodules/.data-custom/configurations/features.json
            if (!SaveFeaturesAndReload(featuresManagementResponse.Msg.Features)) return false;

            return true;
        }


        #region Helper Functions

        private bool SaveFeaturesAndReload(string features)
        {
            try
            {
                var configurationsPath = Path.Combine(_globalConfiguration.GlobalFolder, FeatureConstants.FeaturesPath);

                if (!Directory.Exists(configurationsPath)) 
                    Directory.CreateDirectory(configurationsPath);

                var featureFilePath = Path.Combine(configurationsPath, FeatureConstants.FeaturesJson);

                File.WriteAllText(featureFilePath, features);
                _systemLoader.Reload();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #endregion
    }
}
