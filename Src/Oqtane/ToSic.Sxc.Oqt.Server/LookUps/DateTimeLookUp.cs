using System;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class DateTimeLookUp : LookUpBase
    {
        public DateTimeLookUp()
        {
            Name = "DateTime";
        }

        public override string Get(string key, string format)
        {
            return key.ToLowerInvariant() switch
            {
                "now" => DateTime.Now.ToString(format),

                // todo: STV - also add "System" and "UTC" as documented here 
                // http://www.dnnsoftware.com/Content/Dnn.Platform/Documentation/Using%20Common%20Tools/Replacement%20Tokens/List%20of%20Replacement%20Tokens.html
                _ => string.Empty
            };
        }
    }
}