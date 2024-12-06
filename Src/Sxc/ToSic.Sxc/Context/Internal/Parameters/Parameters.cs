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
/// If any parameter (like 'something') is dynamic, the second Set(...) would fail, because it can't find the method on `object`.
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial record Parameters : IParameters
{
    /// <summary>
    /// Initial NVC, but null-corrected.
    /// This is the only place where the initial NVC is stored.
    /// </summary>
    [PrivateApi]
    public NameValueCollection Nvc { get => field ??= []; init; }

    #region Get (new v15.04)

    /// <summary>
    /// All access to parameters should run through this, so we can determine which keys are used
    /// to determine how the cache should behave / vary.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>
    /// DO NOT use this method for checking if a key exists, as it will log the key as used.
    /// </remarks>
    private bool TryGetAndLog(string name, out string value)
    {
        if (!OriginalsAsDic.TryGetValue(name, out value)) return false;
        UsedKeys.Add(name.ToLowerInvariant());
        return true;
    }

    /// <summary>
    /// The used keys - only lower-cased keys may be added.
    /// </summary>
    internal HashSet<string> UsedKeys = [];

    public string Get(string name)
        => TryGetAndLog(name, out var value) ? value : null;

    public TValue Get<TValue>(string name)
        => GetV<TValue>(name, noParamOrder: default, fallback: default);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default) 
        => GetV(name, noParamOrder, fallback);

    TValue ITyped.Get<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, string language) 
        => GetV(name, noParamOrder, fallback);

    private TValue GetV<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required = default, [CallerMemberName] string cName = default)
        => TryGetAndLog(name, out var value)
            ? value.ConvertOrFallback(fallback)
            : required ?? false
                ? throw new ArgumentException($"Can't find {name} and {nameof(required)} is true; use {nameof(required)}: false if this is intended")
                : fallback;

    #endregion

    private IReadOnlyDictionary<string, string> OriginalsAsDic {
        get
        {
            if (field != null) return field;

            // var temp = initialNvc

            var newDic = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var key in Nvc.Keys)
            {
                // key is usually as string, but sometimes it's null
                // we're not sure if DNN is breaking this, or if it should really be like this
                if (key is string stringKey)
                    newDic[stringKey] = Nvc[stringKey];
                else
                {
                    var nullValues = Nvc[null];
                    if (nullValues == null) continue;
                    foreach (var nullKey in nullValues.CsvToArrayWithoutEmpty())
                        if (!newDic.ContainsKey(nullKey))
                            newDic[nullKey] = null;
                }
            }
            return field = newDic;
        }
    }

    #region Basic Dictionary Properties

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => OriginalsAsDic.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => OriginalsAsDic.GetEnumerator();

    public int Count => OriginalsAsDic.Count;

    public bool ContainsKey(string key) => OriginalsAsDic.ContainsKey(key);

    public bool TryGetValue(string key, out string value) => TryGetAndLog(key, out value);

    public string this[string name] => Get(name);

    public IEnumerable<string> Keys => OriginalsAsDic.Keys;

    public IEnumerable<string> Values => OriginalsAsDic.Values;

    #endregion

    public IParameters Prioritize(string fields = default) =>
        new Parameters(this)
        {
            PriorityFields = fields
        };

    private string PriorityFields { get; init; }

    public override string ToString() => _toString ??= ToString(sort: true);
    private string _toString;

    /// <summary>
    /// Special sorted ToString - for the moment not public
    /// </summary>
    /// <param name="protector"></param>
    /// <param name="sort"></param>
    /// <returns></returns>
    internal string ToString(NoParamOrder protector = default, bool sort = false)
        => sort ? _sorted ??= Nvc.Sort(PriorityFields).NvcToString() : Nvc.NvcToString();
    private string _sorted;


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
        // Only exit early if null. If empty string, basically drop the list.
        if (names == null)
            return this;

        if (names.IsEmptyOrWs())
            return new Parameters(this) { Nvc = [] };

        var keysToKeep = names.CsvToArrayWithoutEmpty();

        var removeKeys = Nvc.AllKeys
            .Where(k => !keysToKeep.Contains(k, StringComparer.InvariantCultureIgnoreCase))
            .ToList();
        var copy = new NameValueCollection(Nvc);
        foreach (var k in removeKeys)
            copy.Remove(k);
        return new Parameters { Nvc = copy };
    }

    public IParameters Flush() => new Parameters(this) { Nvc = [] };

    #endregion

    #region Basic Add/Set/Remove with key only or string-value

    public IParameters Add(string key)
    {
        var copy = new NameValueCollection(Nvc) { { key, null } };
        return new Parameters { Nvc = copy };
    }

    public IParameters Add(string key, string value)
    {
        var copy = new NameValueCollection(Nvc) { { key, value } };
        return new Parameters { Nvc = copy };
    }

    public IParameters Set(string name, string value)
    {
        var copy = new NameValueCollection(Nvc);
        copy.Set(name, value);
        return new Parameters { Nvc = copy };
    }

    public IParameters Set(string name) => Set(name, null);

    public IParameters Remove(string name)
    {
        // Skip cloning if the key doesn't exist or is already null
        if (Nvc[name] == null) return this;

        var copy = new NameValueCollection(Nvc);
        copy.Remove(name);
        return new Parameters { Nvc = copy };
    }

    private IParameters Remove(string name, string value)
    {
        var values = Nvc.GetValues(name);
        if (values == null || values.Length == 0) return this;

        var copy = new NameValueCollection(Nvc);
        copy.Remove(name);
        var rest = values.Where(v => !v.EqualsInsensitive(value)).ToList();
        if (rest.Count == 0)
            return new Parameters { Nvc = copy };

        foreach (var r in rest)
            copy.Add(name, r);
        return new Parameters { Nvc = copy };
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

    /// <summary>
    /// Get by name should never throw an error, as it's used to get null if not found.
    /// </summary>
    object ICanGetByName.Get(string name) => (this as ITyped).Get(name, required: false);

}