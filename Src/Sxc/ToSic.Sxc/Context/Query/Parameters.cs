using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context.Query
{
    /// <summary>
    /// This should provide cross-platform, neutral way to have page parameters in the Razor
    /// </summary>
    [PrivateApi]
    public class Parameters : IReadOnlyDictionary<string, string>
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
                foreach (var key in _originalsAsDic.Keys) 
                    _originalsAsDic[key] = _originals[key];
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
    }
}
