using System.Runtime.CompilerServices;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Typed;
using static ToSic.Sxc.Data.Internal.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Internal.Wrapper;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class PreWrapBase(object data) : IWrapper<object>, IHasJsonSource
{
    #region IWrapper

    object IWrapper<object>.GetContents() => data;

    #endregion

    public abstract WrapperSettings Settings { get; }

    #region Abstract things which must be implemented like TryGetWrap

    public abstract object JsonSource();

    public abstract TryGetResult TryGetWrap(string name, bool wrapDefault = true);

    public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

    #endregion

    #region Abstract: Keys

    public abstract IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default);

    public abstract bool ContainsKey(string name);

    #endregion

    #region TryGet and FindPropertyInternals

    public object TryGetObject(string name, NoParamOrder noParamOrder, bool? required, [CallerMemberName] string cName = default)
    {
        var result = TryGetWrap(name, true);
        return IsErrStrict(result.Found, required, Settings.PropsRequired)
            ? throw ErrStrict(name, cName: cName)
            : result.Result;
    }

    public TValue TryGetTyped<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default)
    {
        var result = TryGetWrap(name, false);
        return IsErrStrict(result.Found, required, Settings.PropsRequired)
            ? throw ErrStrict(name, cName: cName)
            : result.Result.ConvertOrFallback(fallback);
    }

    /// <inheritdoc />
    [PrivateApi("Internal")]
    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        path = path.KeepOrNew().Add($"PreWrap.{GetType()}", specs.Field);
        var result = TryGetWrap(specs.Field, true).Result;
        return new(result: result, valueType: ValueTypesWithState.Dynamic, path: path) { Source = this, Name = "dynamic" };
    }

    #endregion

}