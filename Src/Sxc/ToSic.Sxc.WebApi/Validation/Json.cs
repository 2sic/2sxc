using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToSic.Sxc.WebApi.Validation
{
    internal class Json
    {
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

            // json is valid
            return true;
        }
    }
}
