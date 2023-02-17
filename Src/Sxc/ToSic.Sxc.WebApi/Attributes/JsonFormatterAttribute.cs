using System;

namespace ToSic.Sxc.WebApi
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class JsonFormatterAttribute : Attribute
    {
        public Casing Casing { get; set; } = Casing.CamelCase;
        public bool AutoConvertEntity { get; set; } = true;
        public JsonFormatterAttribute()
        { }
    }

    public enum Casing
    {
        CamelCase, // camelCase
        PascalCase
    }
}
