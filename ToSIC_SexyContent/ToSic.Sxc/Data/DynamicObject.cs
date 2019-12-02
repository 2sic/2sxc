using System;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Case insensitive wrapper for the dynamic object
    /// </summary>
    [PrivateApi("may be moved to EAV or something, so don't publish it here yet...")]
    public class DynamicObject: System.Dynamic.DynamicObject
    {
        public readonly JObject OriginalData;

        public DynamicObject(JObject originalData)
        {
            OriginalData = originalData;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!OriginalData.HasValues)
            {
                result = null;
                return false;
            }

            var found = OriginalData.Properties()
                .FirstOrDefault(p => string.Equals(p.Name, binder.Name, StringComparison.InvariantCultureIgnoreCase));

            if(found != null)
            {
                var original = found.Value;
                result = WrapOrTypeResult(original);
                return true;
            }

            // not found
            result = null;
            return false;
        }

        private static object WrapOrTypeResult(JToken original)
        {
            switch (original)
            {
                case JArray jArray:
                    return jArray.Select(WrapOrTypeResult).ToArray();
                case JObject jResult: // it's another complex object, so return another wrapped reader
                    return new DynamicObject(jResult);
                case JValue jValue: // it's a simple value - so we want to return the underlying real value
                    return jValue.Value;
                default: // it's something else, let's just return that
                    return original;
            }
        }
    }
}
