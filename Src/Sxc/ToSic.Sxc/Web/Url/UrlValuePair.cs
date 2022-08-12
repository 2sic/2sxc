using System;

namespace ToSic.Sxc.Web.Url
{
    /// <summary>
    /// Helper class to prepare data for use in a url parameter.
    /// Especially useful to ensure that the value part is encoded, but not re-encoded.
    /// </summary>
    internal class UrlValuePair
    {
        public UrlValuePair(string name, string value, bool isEncoded = false)
        {
            Name = name;
            Value = value;
            IsEncoded = isEncoded;
        }
        public string Name { get; }
        public string Value { get; }
        public bool IsEncoded { get; }

        public override string ToString()
        {
            var start = Name != null ? Name + "=" : null;
            var val = IsEncoded ? Value : Uri.EscapeUriString(Value);
            return $"{start}{val}";
        }

    }
}
