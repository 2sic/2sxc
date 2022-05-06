using System;

namespace ToSic.Sxc.Oqt.Shared.Models
{
    /// <summary>
    /// Used to transfer what / how page properties should change based on the Razor file
    /// </summary>
    public struct OqtPagePropertyChanges
    {
        public OqtPagePropertyOperation Change { get; set; }
        public OqtPageProperties Property { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }



        /// <summary>
        /// If new value has placeholder token [original], token will be replaced
        /// with old value, effectively injecting old value in new value
        /// </summary>
        /// <param name="original">old value</param>
        public OqtPagePropertyChanges InjectOriginalInValue(string original)
        {
            if (string.IsNullOrEmpty(Value) || !Value.Contains(OriginalToken, StringComparison.OrdinalIgnoreCase))
                return this;

            Value = Value?.Replace(OriginalToken, original ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            Change = OqtPagePropertyOperation.Replace;

            return this;
        }

        public const string OriginalToken = "[original]"; // new value can have [original] placeholder to inject old value in that position
    }

    public enum OqtPageProperties
    {
        Title,
        Keywords,
        Description,
        Base
    }

    public enum OqtPagePropertyOperation
    {
        Replace,
        ReplaceOrSkip,
        Prefix,
        Suffix
    }
}
