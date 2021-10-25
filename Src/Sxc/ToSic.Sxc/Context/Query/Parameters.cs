using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Web.Url;

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


        public override string ToString() => UrlHelpers.NvcToString(_originals);

        public IParameters Add(string name)
        {
            var copy = new NameValueCollection(_originals) { { name, null } };
            return new Parameters(copy);
        }

        public IParameters Add(string name, string value)
        {
            var copy = new NameValueCollection(_originals) { { name, value } };
            return new Parameters(copy);
        }

        public IParameters Set(string name, string value)
        {
            var copy = new NameValueCollection(_originals);
            copy.Set(name, value);
            return new Parameters(copy);
        }

        public IParameters Set(string name)
        {
            var copy = new NameValueCollection(_originals);
            copy.Set(name, null);
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
