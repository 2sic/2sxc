using System;
using Newtonsoft.Json;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a serializer-helper which Newtonsoft will pick up automatically when converting a DynamicJacket to JSON
    /// </summary>
    public class DynamicJacketJsonConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is DynamicJacket dynJacket))
                throw new ArgumentException($"Object should be a dynJacket", nameof(value));

            serializer.Serialize(writer, dynJacket.UnwrappedContents);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(DynamicJacket);
    }
}
