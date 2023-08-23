using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.Url;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Context.Query
{
    /// <summary>
    /// This should provide cross-platform, neutral way to have page parameters in the Razor
    /// </summary>
    [PrivateApi("Hide implementation")]
    public partial class Parameters : IParameters
    {
        #region Constructor

        public Parameters() : this(null) { }

        public Parameters(NameValueCollection originals)
        {
            Nvc = originals ?? new NameValueCollection();
        }

        protected readonly NameValueCollection Nvc;

        #endregion

        #region Get (new v15.04)

        public string Get(string name) => OriginalsAsDic.TryGetValue(name, out var value) ? value : null;

        public TValue Get<TValue>(string name) => GetV<TValue>(name, noParamOrder: Protector, fallback: default);

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default) 
            => GetV(name, noParamOrder, fallback);

        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required) 
            => GetV(name, noParamOrder, fallback);

        private TValue GetV<TValue>(string name, string noParamOrder, TValue fallback, bool? required = default, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            return OriginalsAsDic.TryGetValue(name, out var value)
                ? value.ConvertOrFallback(fallback)
                : (required ?? false)
                    ? throw new ArgumentException($"Can't find {name} and {nameof(required)} is true; use {nameof(required)}: false if this is intended")
                    : fallback;
        }

        #endregion

        private IDictionary<string, string> OriginalsAsDic {
            get
            {
                if (_originalsAsDic != null) return _originalsAsDic;
                _originalsAsDic = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var key in Nvc.Keys)
                {
                    // key is usually as string, but sometimes it's null
                    // we're not sure if DNN is breaking this, or if it should really be like this
                    if (key is string stringKey)
                        _originalsAsDic[stringKey] = Nvc[stringKey];
                    else
                    {
                        var nullValues = Nvc[null];
                        if (nullValues == null) continue;
                        foreach (var nullKey in nullValues.Split(','))
                            if(!string.IsNullOrEmpty(nullKey) && !_originalsAsDic.ContainsKey(nullKey))
                                _originalsAsDic[nullKey] = null;
                    }
                }
                return _originalsAsDic;
            }
        }
        private IDictionary<string, string> _originalsAsDic;

        #region Basic Dictionary Properties

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => OriginalsAsDic.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => OriginalsAsDic.GetEnumerator();

        public int Count => OriginalsAsDic.Count;

        public bool ContainsKey(string key) => OriginalsAsDic.ContainsKey(key);

        public bool TryGetValue(string key, out string value) => OriginalsAsDic.TryGetValue(key, out value);

        public string this[string name] => Get(name);

        public IEnumerable<string> Keys => OriginalsAsDic.Keys;

        public IEnumerable<string> Values => OriginalsAsDic.Values;

        #endregion


        public override string ToString() => Nvc.NvcToString();

        #region Basic Add/Set/Remove with key only or string-value

        public IParameters Add(string key)
        {
            var copy = new NameValueCollection(Nvc) { { key, null } };
            return new Parameters(copy);
        }

        public IParameters Add(string key, string value)
        {
            var copy = new NameValueCollection(Nvc) { { key, value } };
            return new Parameters(copy);
        }

        public IParameters Set(string name, string value)
        {
            var copy = new NameValueCollection(Nvc);
            copy.Set(name, value);
            return new Parameters(copy);
        }

        public IParameters Set(string name) => Set(name, null);

        public IParameters Remove(string name)
        {
            var copy = new NameValueCollection(Nvc);
            if (copy[name] != null)
                copy.Remove(name);
            return new Parameters(copy);
        }

        #endregion


        #region New Object Add/Set

        public IParameters Add(string key, object value) => Add(key, ValueToUrlValue(value));

        public IParameters Set(string name, object value) => Set(name, ValueToUrlValue(value));

        private string ValueToUrlValue(object value)
        {
            if (value is null) return null;
            if (value is bool bVal) return bVal.ToString().ToLower();
            if (value.IsNumeric()) return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            if (value is DateTime dtmVal)
            {
                var result = System.DateTime.SpecifyKind(dtmVal, DateTimeKind.Utc).ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                // if the time is zero, trim that
                if (result.EndsWith("T00:00:00")) return result.Substring(0, result.IndexOf('T'));
                return result;
            }
            return value.ToString();
        }

        #endregion
        
    }
}
