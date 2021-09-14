using System;
using Newtonsoft.Json;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a serializer-helper which Newtonsoft will pick up automatically when converting a DynamicJacket to JSON
    /// </summary>
    public class DynamicJsonConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is IHasJsonSource dynJacket))
                throw new ArgumentException($"Object should be a dynJacket", nameof(value));

            serializer.Serialize(writer, dynJacket.JsonSource);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(DynamicJacket);
    }
}
