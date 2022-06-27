using System;
using System.Collections;
using System.Linq;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Url
{
    public class ObjectToUrl
    {
        public ObjectToUrl(string prefix = null)
        {
            Prefix = prefix;
        }

        public string Prefix { get; }
        public string ArraySeparator { get; set; } = ",";
        public string DepthSeparator { get; set; } = ":";
        public string PairSeparator { get; set; } = UrlParts.ValuePairSeparator.ToString();

        public string KeyValueSeparator { get; set; } = "=";


        public string Serialize(object data) => Serialize(data, Prefix);


        public string SerializeIfNotString(object data, string prefix = null)
        {
            if (data == null) return null;
            if (data is string str) return str;
            return Serialize(data, prefix);
        }


        public string SerializeWithChild(object main, object child, string childPrefix)
        {
            var uiString = SerializeIfNotString(main);
            if (child == null) return uiString;
            var prefillAddOn = "";
            if (child is string strPrefill)
            {
                var parts = strPrefill.Split(UrlParts.ValuePairSeparator)
                    .Where(p => p.HasValue())
                    .Select(p => p.StartsWith(childPrefix) ? p : childPrefix + p);
                prefillAddOn = string.Join(UrlParts.ValuePairSeparator.ToString(), parts);
            }
            else
                prefillAddOn = SerializeIfNotString(child, childPrefix);

            return UrlParts.ConnectParameters(uiString, prefillAddOn);
        }



        private ValuePair ValueSerialize(object value, string propName)
        {
            if (value == null) return new ValuePair(propName, null);
            if (value is string strValue) return new ValuePair(propName, strValue);

            var valueType = value.GetType();

            // Check array - not sure yet if we care
            if (value is IEnumerable enumerable)
            {
                var valueElemType = valueType.IsGenericType
                    ? valueType.GetGenericArguments()[0]
                    : valueType.GetElementType();

                if (valueElemType == null) throw new ArgumentNullException("The type to add to url seems to have a confusing setup");
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                    return new ValuePair(propName, string.Join(ArraySeparator, enumerable.Cast<object>()));

                return new ValuePair(propName, "array-like-but-unclear-what");
            }

            return valueType.IsSimpleType() 
                ? new ValuePair(propName, value.ToString()) 
                : new ValuePair(null, Serialize(value, propName + DepthSeparator), true);
        }

        // https://ole.michelsen.dk/blog/serialize-object-into-a-query-string-with-reflection/
        // https://stackoverflow.com/questions/6848296/how-do-i-serialize-an-object-into-query-string-format
        public string Serialize(object objToConvert, string prefix)
        {
            if (objToConvert == null)
                throw new ArgumentNullException(nameof(objToConvert));

            // Get all properties on the object
            var properties = objToConvert.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Select(x => ValueSerialize(x.GetValue(objToConvert, null), prefix + x.Name))
                .Where(x => x.Value != null)
                .ToList();

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join(PairSeparator, properties.Select(p => p.ToString()));

        }

        private class ValuePair
        {
            public ValuePair(string name, string value, bool isEncoded = false)
            {
                Name = name;
                Value = value;
                IsEncoded = isEncoded;
            }
            public string Name { get; }
            public string Value { get; }
            public bool IsEncoded { get; }

            public override string ToString()
            {
                var start = Name != null ? Name + "=" : null;
                var val = IsEncoded ? Value : Uri.EscapeUriString(Value);
                return $"{start}{val}";
            }
        }
    }
}
