using System.Net;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Data.Shared;
using ToSic.Lib.Data;
using ToSic.Lib.DI;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Data.Internal.Typed;

[PrivateApi]
[JsonConverter(typeof(DynamicJsonConverter))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WrapObjectTyped(LazySvc<IScrub> scrubSvc, LazySvc<ConvertForCodeService> forCodeConverter)
    : IWrapper<IPreWrap>, ITyped, IHasPropLookup, IHasJsonSource
{
    internal IPreWrap PreWrap { get; private set; }

    internal WrapObjectTyped Setup(IPreWrap preWrap)
    {
        PreWrap = preWrap;
        return this;
    }

    IPreWrap IWrapper<IPreWrap>.GetContents() => PreWrap;

    IPropertyLookup IHasPropLookup.PropertyLookup => PreWrap;

    #region Keys

    public bool ContainsKey(string name) => TypedHelpers.ContainsKey(name, this,
        (e, k) => e.PreWrap.ContainsKey(k),
        (e, k) =>
        {
            var child = e.PreWrap.TryGetWrap(k);
            if (!child.Found || child.Result == null) return null;
            if (child.Result is WrapObjectTyped typed) return typed;
            // Note: arrays have never been supported, so the following won't work
            // Because the inner objects are not of the expected type.
            // We don't want to start supporting it now.
            // Leave this code in though, so we know that we did try it.
            //if (child.Result is IEnumerable list)
            //    return list.Cast<WrapObjectTyped>().FirstOrDefault(o => o != null);
            return null;
        }
    );

    public bool IsEmpty(string name, NoParamOrder noParamOrder = default, string language = default /* ignore */)
        => HasKeysHelper.IsEmpty(this, name, noParamOrder, default);

    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string language = default /* ignore */)
        => HasKeysHelper.IsNotEmpty(this, name, noParamOrder, default);



    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default)
        => PreWrap.Keys(noParamOrder, only);

    #endregion

    #region ITyped Get

    object ITyped.Get(string name, NoParamOrder noParamOrder, bool? required, string language /* ignore */)
        => PreWrap.TryGetObject(name, noParamOrder, required);

    TValue ITyped.Get<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, string language /* note ignored */)
        => PreWrap.TryGetTyped(name, noParamOrder, fallback, required: required);

    #endregion

    #region ITyped Values: Bool, String, etc.

    bool ITyped.Bool(string name, NoParamOrder noParamOrder, bool fallback, bool? required)
        => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    DateTime ITyped.DateTime(string name, NoParamOrder noParamOrder, DateTime fallback, bool? required)
        => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    string ITyped.String(string name, NoParamOrder noParamOrder, string fallback, bool? required, object scrubHtml)
    {
        var value = PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);
        return TypedItemHelpers.MaybeScrub(value, scrubHtml, () => scrubSvc.Value);
    }

    int ITyped.Int(string name, NoParamOrder noParamOrder, int fallback, bool? required)
        => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    long ITyped.Long(string name, NoParamOrder noParamOrder, long fallback, bool? required)
        => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    float ITyped.Float(string name, NoParamOrder noParamOrder, float fallback, bool? required)
        => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    decimal ITyped.Decimal(string name, NoParamOrder noParamOrder, decimal fallback, bool? required)
        => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    double ITyped.Double(string name, NoParamOrder noParamOrder, double fallback, bool? required)
        => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    #endregion

    #region ITyped Specials: Attribute, Url

    string ITyped.Url(string name, NoParamOrder noParamOrder, string fallback, bool? required)
    {
        var url = PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback, required: required);
        return Tags.SafeUrl(url).ToString();
    }

    IRawHtmlString ITyped.Attribute(string name, NoParamOrder noParamOrder, string fallback, bool? required)
    {
        var value = PreWrap.TryGetWrap(name, false).Result;
        var strValue = forCodeConverter.Value.ForCode(value, fallback: fallback);
        return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
    }

    #endregion

    #region Equality

    /// <summary>
    /// This is used by various equality comparison. 
    /// Since we define two object to be equal when they host the same contents, this determines the hash based on the contents
    /// </summary>
    [PrivateApi]
    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => WrapperEquality.GetWrappedHashCode(PreWrap);

    public override bool Equals(object b)
    {
        if (b is null) return false;
        if (ReferenceEquals(this, b)) return true;
        if (b.GetType() != GetType()) return false;
        return WrapperEquality.EqualsWrapper(PreWrap, ((WrapObjectTyped)b).PreWrap);
    }

    #endregion

    #region Explicit interfaces for Json, PropertyLookup etc.

    [PrivateApi]
    object IHasJsonSource.JsonSource() => PreWrap.JsonSource();

    #endregion

    #region ToString to support Json objects

    public override string ToString() => PreWrap.GetContents().ToString();

    #endregion

    /// <summary>
    /// Get by name should never throw an error, as it's used to get null if not found.
    /// </summary>
    object ICanGetByName.Get(string name) => (this as ITyped).Get(name, required: false);

}