using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.Validation;


// Todo: MVC - has a DNN folder name in this code
// must be injected from elsewhere

namespace ToSic.Sxc.WebApi.Features
{
    public class FeaturesBackend: WebApiBackendBase<FeaturesBackend>
    {
        #region Constructor / DI

        public FeaturesBackend(IServerPaths serverPaths, IZoneMapper zoneMapper, IServiceProvider serviceProvider) : base(serviceProvider, "Bck.Feats")
        {
            _serverPaths = serverPaths;
            _zoneMapper = zoneMapper;
        }

        private readonly IServerPaths _serverPaths;
        private readonly IZoneMapper _zoneMapper;

        public new FeaturesBackend Init(ILog parentLog)
        {
            base.Init(parentLog);
            _zoneMapper.Init(Log);
            return this;
        }

        #endregion

        public IEnumerable<Feature> GetAll(bool reload)
        {
            if (reload) Eav.Configuration.Features.Reset();
            return Eav.Configuration.Features.All;
        }


        public bool SaveFeatures(FeaturesDto featuresManagementResponse)
        {
            // first do a validity check 
            if (featuresManagementResponse?.Msg?.Features == null) return false;

            // 1. valid json? 
            // - ensure signature is valid
            if (!IsValidFeaturesJson(featuresManagementResponse.Msg.Features)) return false;

            // then take the newFeatures (it should be a json)
            // and save to /desktopmodules/.data-custom/configurations/features.json
            if (!SaveFeature(featuresManagementResponse.Msg.Features)) return false;

            // when done, reset features
            Eav.Configuration.Features.Reset();

            return true;
        }




        #region Helper Functions

        public static bool IsValidFeaturesJson(string input)
        {
            return Json.IsValidJson(input);

            // todo: ensure signature is valid

            // json is valid
            //return true;
        }

        //public static bool IsValidJson(string strInput)
        //{
        //    strInput = strInput.Trim();
        //    if (!(strInput.StartsWith("{") && strInput.EndsWith("}")) &&
        //        !(strInput.StartsWith("[") && strInput.EndsWith("]")))
        //        // it is not js Object and not js Array
        //        return false;

        //    try
        //    {
        //        JToken.Parse(strInput);
        //    }
        //    catch (JsonReaderException)
        //    {
        //        //  exception in parsing json
        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        // some other exception
        //        return false;
        //    }

        //    // json is valid
        //    return true;
        //}

        public bool SaveFeature(string features)
        {
            try
            {
                var configurationsPath = _serverPaths.FullAppPath("~/DesktopModules/ToSIC_SexyContent/" + Eav.Configuration.Features.FeaturesPath);

                if (!Directory.Exists(configurationsPath)) 
                    Directory.CreateDirectory(configurationsPath);

                var featureFilePath = Path.Combine(configurationsPath, Eav.Configuration.Features.FeaturesJson);

                File.WriteAllText(featureFilePath, features);
                Eav.Configuration.Features.Reset();
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
