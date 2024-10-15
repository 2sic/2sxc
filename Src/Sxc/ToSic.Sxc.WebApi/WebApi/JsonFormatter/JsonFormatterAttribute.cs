namespace ToSic.Sxc.WebApi;

/// <summary>
/// Mark a WebApi to use the modern Json Formatter based on System.Text.Json.
/// Without this, older WebApi Controllers use the Newtonsoft JSON Formatter.
/// Also provides additional configuration to make certain work easier. 
/// </summary>
/// <remarks>
/// * new in v15.08
/// </remarks>
[PublicApi]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JsonFormatterAttribute : Attribute
{
    /// <summary>
    /// Specify how <see cref="IEntity"/> objects in the result should be formatted.
    /// Default is <see cref="WebApi.EntityFormat.Light"/>.
    /// </summary>
    public EntityFormat EntityFormat { get; set; } = EntityFormat.Light;

    /// <summary>
    /// Specify how resulting objects should be cased.
    /// Default is <see cref="WebApi.Casing.Camel"/>.
    /// Will affect both normal object properties as well as Dictionary keys.
    /// </summary>
    public Casing Casing { get; set; } = Casing.Camel;

    [PrivateApi("Hide constructor, not important for docs")]
    public JsonFormatterAttribute() { }
}