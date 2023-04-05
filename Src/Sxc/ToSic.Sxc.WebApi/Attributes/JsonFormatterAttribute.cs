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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JsonFormatterAttribute : Attribute
    {
        /// <summary>
        /// Automatically convert IEntity objects in the result to the preferred object structure. 
        /// </summary>
        // @STV TODO: - switch to EntityFormat
        public bool AutoConvertEntity { get; set; } = true;

        public EntityFormats EntityFormat { get; set; } = EntityFormats.Light;


        public Casing Casing { get; set; } = Casing.CamelCase;

        [PrivateApi("Hide constructor, not important for docs")]
        public JsonFormatterAttribute() { }
    }

    /// <summary>
    /// Determines what casing to use when converting data to JSON
    /// </summary>
    public enum Casing
    {
        /// <summary>
        /// Leave casing as is.
        /// Currently not supported, will behave as CamelCase for now.
        /// </summary>
        Default,

        /// <summary>
        /// Set casing to use CamelCase for Attributes
        /// </summary>
        CamelCase,

        /// <summary>
        /// Set casing to use pascalCase for Attributes.
        /// </summary>
        PascalCase
    }

    /// <summary>
    /// Formats to use for automatic Entity to JSON conversion.
    /// </summary>
    public enum EntityFormats
    {
        /// <summary>
        /// Do not auto-convert into any specific format.
        /// If <see cref="IEntity"/> objects are in the result, will throw an error.
        /// </summary>
        None,

        /// <summary>
        /// Format <see cref="IEntity"/> objects as <see cref="EavLightEntity"/>.
        /// This results in single-language objects with name/value pairs like a JavaScript object.
        /// </summary>
        Light,
    }
}
