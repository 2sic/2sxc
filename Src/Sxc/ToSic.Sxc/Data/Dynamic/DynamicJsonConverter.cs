using System;
using Newtonsoft.Json;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a serializer-helper which Newtonsoft will pick up automatically when converting a DynamicJacket or DynamicReadObject to JSON
    /// </summary>
    public class DynamicJsonConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is IHasJsonSource hasJsonSource))
                throw new ArgumentException($"Object should be a {nameof(IHasJsonSource)}", nameof(value));

            serializer.Serialize(writer, hasJsonSource.JsonSource);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) 
            => throw new NotSupportedException();

        public override bool CanConvert(Type objectType) => objectType == typeof(IHasJsonSource);
    }
}
