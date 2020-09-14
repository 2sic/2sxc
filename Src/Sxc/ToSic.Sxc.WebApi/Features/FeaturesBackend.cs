using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Web;


// Todo: MVC - has a DNN folder name in this code
// must be injected from elsewhere

namespace ToSic.Sxc.WebApi.Features
{
    public class FeaturesBackend: WebApiBackendBase<FeaturesBackend>
    {
        #region Constructor / DI

        public FeaturesBackend(IHttp http, IZoneMapper zoneMapper) : base("Bck.Feats")
        {
            _http = http;
            _zoneMapper = zoneMapper;
        }
        private readonly IHttp _http;
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

        // 2020-09-14 2dm disabled this - don't think it's in use
        //public IEnumerable<Feature> Features(IInstanceContext context, int tenantId, int appId)
        //{
        //    // some dialogs don't have an app-id yet, because no app has been selected
        //    // in this case, use the app-id of the content-app for feature-permission check
        //    if (appId == Eav.Constants.AppIdEmpty)
        //    {
        //        var zoneId = _zoneMapper.GetZoneId(tenantId);
        //        appId = new ZoneRuntime(zoneId, Log).DefaultAppId;
        //    }

        //    return FeaturesHelpers.FeatureListWithPermissionCheck(new MultiPermissionsApp().Init(context, GetApp(appId, null), Log));
        //}


        public bool SaveFeatures(FeaturesDto featuresManagementResponse)
        {
            // first do a validity check 
            if (featuresManagementResponse?.Msg?.Features == null) return false;

            // 1. valid json? 
            // - ensure signature is valid
            if (!IsValidJson(featuresManagementResponse.Msg.Features)) return false;

            // then take the newFeatures (it should be a json)
            // and save to /desktopmodules/.data-custom/configurations/features.json
            if (!SaveFeature(featuresManagementResponse.Msg.Features)) return false;

            // when done, reset features
            Eav.Configuration.Features.Reset();

            return true;
        }




        #region Helper Functions



        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if (!(strInput.StartsWith("{") && strInput.EndsWith("}")) &&
                !(strInput.StartsWith("[") && strInput.EndsWith("]")))
                // it is not js Object and not js Array
                return false;

            try
            {
                JToken.Parse(strInput);
            }
            catch (JsonReaderException)
            {
                //  exception in parsing json
                return false;
            }
            catch (Exception)
            {
                // some other exception
                return false;
            }

            // todo: ensure signature is valid

            // json is valid
            return true;
        }

        public bool SaveFeature(string features)
        {
            try
            {
                var configurationsPath = _http.MapPath("~/DesktopModules/ToSIC_SexyContent/" + Eav.Configuration.Features.FeaturesPath);

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
