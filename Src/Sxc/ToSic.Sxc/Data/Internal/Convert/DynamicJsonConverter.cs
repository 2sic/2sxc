using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ToSic.Sxc.Data.Internal.Convert;

/// <summary>
/// This is a serializer-helper which System.Text.Json will pick up automatically when converting a DynamicJacket or DynamicReadObject to JSON
/// </summary>
/// <remarks>
///https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-6-0
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DynamicJsonConverter: JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        => JsonSerializer.Deserialize(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is not IHasJsonSource hasJsonSource)
            throw new ArgumentException($"Object should be a {nameof(IHasJsonSource)}", nameof(value));

        JsonSerializer.Serialize(writer, hasJsonSource.JsonSource(), options);
    }

    public override bool CanConvert(Type objectType) =>
        objectType == typeof(IHasJsonSource)
        || typeof(IHasJsonSource).IsAssignableFrom(objectType)
        || objectType == typeof(ITyped)
        || typeof(ITyped).IsAssignableFrom(objectType); // we'll have to keep an eye on it for scenarios where ITypedItem also inherits from ITypedRead, and could have some surprises,
    // but since the DynamicEntity was never meant to become json, probably there is no code out there that tries to do this. 
}