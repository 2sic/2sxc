using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToSic.SexyContent.WebApi
{
    public class FeaturesManagementUtils
    {
        public class FeaturesManagementResponse
        {
            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("msg")]
            public Msg Msg { get; set; }
        }

        public class Msg
        {
            [JsonProperty("features")]
            public string Features { get; set; }

            [JsonProperty("signature")]
            public string Signature { get; set; }
        }

        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if (!(strInput.StartsWith("{") && strInput.EndsWith("}")) &&
                !(strInput.StartsWith("[") && strInput.EndsWith("]")))
            {
                // it is not js Object and not js Array
                return false;
            }

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

            // todo: stv
            // ensure signature is valid

            // json is valid
            return true;
        }

        public static bool SaveFeature(string features)
        {
            bool fileSaved;




            try
            {
                var configurationsPath = HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/.data-custom/configurations/");

                if (!Directory.Exists(configurationsPath))
                {
                    Directory.CreateDirectory(configurationsPath);
                }

                var featureFilePath = Path.Combine(configurationsPath, "features.json");

                File.WriteAllText(featureFilePath, features);
                fileSaved = true;
            }
            catch (Exception)
            {
                // throw;
                fileSaved = false;
            }

            return fileSaved;
        }
    }
}
