using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var obj = JToken.Parse(strInput);
            }
            catch (JsonReaderException jex)
            {
                //  exception in parsing json
                return false;
            }
            catch (Exception ex)
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
                string configurationsPath = HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/.data-custom/configurations/");

                if (!Directory.Exists(configurationsPath))
                {
                    Directory.CreateDirectory(configurationsPath);
                }

                string featureFilePath = Path.Combine(configurationsPath, "features.json");

                File.WriteAllText(featureFilePath, features);
                fileSaved = true;
            }
            catch (Exception e)
            {
                // throw;
                fileSaved = false;
            }

            return fileSaved;
        }
    }
}
