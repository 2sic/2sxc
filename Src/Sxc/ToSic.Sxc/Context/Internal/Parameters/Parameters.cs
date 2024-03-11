using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// This should provide cross-platform, neutral way to have page parameters in the Razor
/// </summary>
/// <remarks>
/// This MUST be public, because in dyn-code you could have Parameters.Set("key", something).Set(...).Set(...).
/// If any parameter (eg 'something') is dynamic, the second Set(...) would fail, because it can't find the method on `object`.
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class Parameters(NameValueCollection originals) : IParameters
{
    #region Constructor

    public Parameters() : this(null) { }

    protected readonly NameValueCollection Nvc = originals ?? [];

    #endregion

    #region Get (new v15.04)

    public string Get(string name) => OriginalsAsDic.TryGetValue(name, out var value) ? value : null;

    public TValue Get<TValue>(string name) => GetV<TValue>(name, noParamOrder: default, fallback: default);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default) 
        => GetV(name, noParamOrder, fallback);

    TValue ITyped.Get<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required) 
        => GetV(name, noParamOrder, fallback);

    private TValue GetV<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required = default, [CallerMemberName] string cName = default)
    {
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
                    foreach (var nullKey in nullValues.CsvToArrayWithoutEmpty())
                        if (!_originalsAsDic.ContainsKey(nullKey))
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

    #region Toggle and Filter

    private IParameters Toggle(string name, string value)
    {
        var oldValue = Get(name);
        return oldValue.EqualsInsensitive(value)
            ? Remove(name)
            : Set(name, value);

        // Maybe: implement detailed replace if multiple values exist?
        // most of this kind of already works, but we don't have all edge cases covered
        // since it's not sure if we ever need this, it's not implemented yet
        //var values = Nvc.GetValues(key);

        //if (value == default)
        //{
        //    return Set(key);
        //}

        //if (values == null || values.Length == 0)
        //    return Set(key, value);

        //if (values.Any(v => v.EqualsInsensitive(value)))
        //    return Remove(key);

        //return Set(key, value);
    }

    public IParameters Toggle(string name, object value) => Toggle(name, ValueToUrlValue(value));

    public IParameters Filter(string names)
    {
        if (names == null || names.IsEmptyOrWs()) return this;
        var keysToKeep = names.CsvToArrayWithoutEmpty();

        var oldKeys = Nvc.AllKeys;
        var removeKeys = oldKeys.Where(k => !keysToKeep.Contains(k, StringComparer.InvariantCultureIgnoreCase)).ToList();
        var copy = new NameValueCollection(Nvc);
        foreach (var k in removeKeys) copy.Remove(k);
        return new Parameters(copy);
    }

    #endregion

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
        if (copy[name] != null) copy.Remove(name);
        return new Parameters(copy);
    }

    private IParameters Remove(string name, string value)
    {
        var values = Nvc.GetValues(name);
        if (values == null || values.Length == 0) return this;

        var copy = new NameValueCollection(Nvc);
        copy.Remove(name);
        var rest = values.Where(v => !v.EqualsInsensitive(value)).ToList();
        if (rest.Count == 0) return new Parameters(copy);

        foreach (var r in rest) copy.Add(name, r);
        return new Parameters(copy);
    }

    public IParameters Remove(string name, object value) => Remove(name, ValueToUrlValue(value));

    #endregion


    #region New Object Add/Set

    public IParameters Add(string key, object value) => Add(key, ValueToUrlValue(value));

    public IParameters Set(string name, object value) => Set(name, ValueToUrlValue(value));

    private string ValueToUrlValue(object value)
    {
        if (value is null) return null;
        if (value is string sVal) return sVal;
        if (value is bool bVal) return bVal.ToString().ToLower();
        if (value.IsNumeric()) return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        if (value is DateTime dtmVal)
        {
            var result = DateTime.SpecifyKind(dtmVal, DateTimeKind.Utc).ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            // if the time is zero, trim that
            if (result.EndsWith("T00:00:00")) return result.Substring(0, result.IndexOf('T'));
            return result;
        }
        return value.ToString();
    }

    #endregion
        
}