using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context.Query
{
    /// <summary>
    /// This should provide cross-platform, neutral way to have page parameters in the Razor
    /// </summary>
    [PrivateApi]
    public class Parameters : IParameters, IReadOnlyDictionary<string, string>
    {
        public Parameters(NameValueCollection originals)
        {
            _originals = originals ?? new NameValueCollection();
        }

        private readonly NameValueCollection _originals;

        private IDictionary<string, string> OriginalsAsDic {
            get
            {
                if (_originalsAsDic != null) return _originalsAsDic;
                _originalsAsDic = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var key in _originals.Keys) 
                    _originalsAsDic[key.ToString()] = _originals[key.ToString()];
                return _originalsAsDic;
            }
        }
        private IDictionary<string, string> _originalsAsDic;
        

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => OriginalsAsDic.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => OriginalsAsDic.GetEnumerator();

        public int Count => OriginalsAsDic.Count;

        public bool ContainsKey(string key) => OriginalsAsDic.ContainsKey(key);

        public bool TryGetValue(string key, out string value) => OriginalsAsDic.TryGetValue(key, out value);

        public string this[string key] => OriginalsAsDic.TryGetValue(key, out var value) ? value : null;

        public IEnumerable<string> Keys => OriginalsAsDic.Keys;

        public IEnumerable<string> Values => OriginalsAsDic.Values;


        public override string ToString()
        {
            var allPairs = _originals.AllKeys
                .Where(k => !string.IsNullOrEmpty(k))
                .SelectMany(k =>
                {
                    var vals = _originals.GetValues(k) ?? new[] { "" }; // catch null-values
                    return vals.Select(v => k + (string.IsNullOrEmpty(v) ? "" : "=" + v));
                })
                .ToArray();
            return allPairs.Any() ? string.Join("&", allPairs) : "";
        }

        public IParameters Add(string name, string value)
        {
            var copy = new NameValueCollection(_originals);
            // Use set, not Add, because NVCs can multiple values per key
            copy.Set(name, value);
            return new Parameters(copy);
        }

        public IParameters Remove(string name)
        {
            var copy = new NameValueCollection(_originals);
            if (copy[name] != null)
                copy.Remove(name);
            return new Parameters(copy);
        }
    }
}
