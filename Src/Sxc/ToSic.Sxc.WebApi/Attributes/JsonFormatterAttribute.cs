using System;
using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.WebApi
{
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
        /// Automatically convert IEntity objects in the result to the preferred object structure. 
        /// </summary>
        // @STV TODO: - switch to EntityFormat
        public bool AutoConvertEntity { get; set; } = true;

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
    /// Can be used as flags, so you can say `Casing = Casing.CamelCase` or `Casing = Casing.ObjectPascal | Casing.DictionaryCamel`
    /// </summary>
    [PublicApi]
    [Flags]
    public enum Casing
    {
        /// <summary>
        /// Leave casing as is.
        /// Currently no real effect, will behave as CamelCase for now.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Set casing to use CamelCase for Attributes.
        /// This is how conversion would have worked before v15, as the C# objects all use CamelCase internally.
        /// </summary>
        Camel = 1 << 0,

        /// <summary>
        /// Set casing to use pascalCase for Attributes.
        /// This is how most modern JavaScript code expects the data to be.
        /// </summary>
        Pascal = 1 << 2,

        DictionaryDefault = 1 << 8,
        DictionaryCamel = 1 << 9,
        DictionaryPascal = 1 << 10,

        ObjectDefault = 1 << 16,
        ObjectCamel = 1 << 17,
        ObjectPascal = 1 << 18,
    }

    /// <summary>
    /// Formats to use for automatic Entity to JSON conversion.
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
}
