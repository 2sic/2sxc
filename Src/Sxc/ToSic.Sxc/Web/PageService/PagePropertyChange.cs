using System;
using ToSic.Sxc.Plumbing;

namespace ToSic.Sxc.Web.PageService
{
    public struct PagePropertyChange
    {
        public PageChangeModes ChangeMode { get; set; }
        
        internal PageProperties Property { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// This is part of the original property, which would be replaced.
        /// </summary>
        public string ReplacementIdentifier { get; set; }

        /// <summary>
        /// If new value has placeholder token [original], token will be replaced
        /// with old value, effectively injecting old value in new value
        /// </summary>
        /// <param name="original">old value</param>
        public PagePropertyChange InjectOriginalInValue(string original) 
        {
            if (string.IsNullOrEmpty(Value) || Value.IndexOf(OriginalToken, StringComparison.OrdinalIgnoreCase) == -1) 
                return this;

            Value = Value.ReplaceIgnoreCase(OriginalToken, original);
            ChangeMode = PageChangeModes.Replace;

            return this;
        }

        private const string OriginalToken = "[original]"; // new value can have [original] placeholder to inject old value in that position
    }
}
