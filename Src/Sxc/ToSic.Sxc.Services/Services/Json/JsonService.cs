using System.Text.Json;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.DynamicJacket;
using ToSic.Sxc.Data.Sys.Wrappers;
using static ToSic.Eav.Serialization.Sys.Json.JsonOptions;

namespace ToSic.Sxc.Services;

[PrivateApi("Hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class JsonService(Generator<CodeJsonWrapper> wrapJsonGenerator)
    : ServiceBase("Sxc.JsnSvc", connect: [wrapJsonGenerator]), IJsonService
{
    /// <inheritdoc />
    public T? To<T>(string json) 
        => JsonSerializer.Deserialize<T>(json, SafeJsonForHtmlAttributes);

    /// <inheritdoc />
    public object? ToObject(string json)
        => JsonSerializer.Deserialize<object>(json, SafeJsonForHtmlAttributes);

    /// <inheritdoc />
    public string ToJson(object item)
        => JsonSerializer.Serialize(item, SafeJsonForHtmlAttributes);

    /// <inheritdoc />
    public string ToJson(object item, int indentation)
        => JsonSerializer.Serialize(item, SafeJsonForHtmlAttributes);

    /// <inheritdoc />
    public ITyped? ToTyped(string json, NoParamOrder npo = default, string? fallback = default, bool? propsRequired = default)
        => wrapJsonGenerator.New()
            .Setup(WrapperSettings.Typed(true, true, propsRequired: propsRequired ?? true))
            .JsonToTyped(json, npo, fallback);


    /// <inheritdoc />
    public IEnumerable<ITyped>? ToTypedList(string json, NoParamOrder npo = default, string? fallback = default, bool? propsRequired = default)
        => wrapJsonGenerator.New()
            .Setup(WrapperSettings.Typed(true, true, propsRequired: propsRequired ?? true))
            .JsonToTypedList(json, npo, fallback);
}