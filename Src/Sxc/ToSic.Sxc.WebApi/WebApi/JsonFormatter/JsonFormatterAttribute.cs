using System;
using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Lib.Documentation;

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

/// <summary>
/// Determines what casing to use when converting data to JSON.
/// This is for the <see cref="JsonFormatterAttribute"/>.
/// Can be used as flags, so you can say `Casing = Casing.CamelCase` or `Casing = Casing.ObjectPascal | Casing.DictionaryCamel`
/// </summary>
[PublicApi]
[Flags]
public enum Casing
{
    /// <summary>
    /// No casing configuration set.
    /// Will preserve casing as it is, usually Pascal case (old behavior for 2sxc Apis).
    /// </summary>
    [PrivateApi("Hidden for now, as it doesn't matter to external users")]
    Unspecified = 0,

    /// <summary>
    /// Set casing to use camelCase for everything.
    /// This is how most JavaScript code expects the data.
    /// The opposite would be <see cref="Preserve"/>.
    /// </summary>
    Camel = 1 << 0,

    /// <summary>
    /// Set casing to use original name for everything - usually PascalCase as is common in C#.
    /// This is how conversion would have worked before v15, as the C# objects all use CamelCase internally.
    /// The opposite would be <see cref="Camel"/>
    /// </summary>
    Preserve = 1 << 2,

    /// <summary>
    /// Set casing of Dictionaries to be camelCase.
    /// For example, Entity properties such as `Birthday` = `birthday`, `FirstName` = `firstName`.
    /// This would be Camel case.
    /// </summary>
    DictionaryCamel = 1 << 9,

    /// <summary>
    /// Set casing of Dictionaries to be PascalCase.
    /// For example, Entity properties such as `Birthday` = `Birthday`, `FirstName` = `firstName`.
    /// This would be Camel case.
    /// </summary>
    DictionaryPreserve = 1 << 10,
}

/// <summary>
/// Formats to use for automatic Entity to JSON conversion.
/// This is for the <see cref="JsonFormatterAttribute"/>.
/// As of now it only has `None` and `Light`, in future we plan to extend this with other formats.
/// Default is usually `Light`.
/// </summary>
[PublicApi]
public enum EntityFormat
{
    /// <summary>
    /// Do not auto-convert into any specific format.
    /// If <see cref="IEntity"/> objects are in the result, will result in an error.
    /// </summary>
    None,

    /// <summary>
    /// Format <see cref="IEntity"/> objects as <see cref="EavLightEntity"/>.
    /// This results in single-language objects with name/value pairs like a JavaScript object.
    /// </summary>
    Light,
}