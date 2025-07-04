﻿using System.Runtime.CompilerServices;
using ToSic.Lib.Wrappers;
using ToSic.Sxc.Data.Sys.Json;
using ToSic.Sxc.Data.Sys.Typed;
using static ToSic.Sxc.Data.Sys.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Sys.Wrappers;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract class PreWrapBase(object? data) : IWrapper<object>, IHasJsonSource
{
    #region IWrapper

    object? IWrapper<object>.GetContents() => data;

    #endregion

    public abstract WrapperSettings Settings { get; }

    #region Abstract things which must be implemented like TryGetWrap

    public abstract object JsonSource();

    public abstract TryGetResult TryGetWrap(string? name, bool wrapDefault = true);

    // #DropUseOfDumpProperties
    //public abstract List<PropertyDumpItem> _DumpNameWipDroppingMostCases(PropReqSpecs specs, string path);

    #endregion

    #region Abstract: Keys

    public abstract IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string>? only = default);

    public abstract bool ContainsKey(string name);

    #endregion

    #region TryGet and FindPropertyInternals

    public object TryGetObject(string name, NoParamOrder noParamOrder, bool? required, [CallerMemberName] string? cName = default)
    {
        var result = TryGetWrap(name, wrapDefault: true);
        return IsErrStrict(result.Found, required, Settings.PropsRequired)
            ? throw ErrStrict(name, cName: cName)
            : result.Result!;
    }

    public TValue? TryGetTyped<TValue>(string name, NoParamOrder noParamOrder, TValue? fallback, bool? required, [CallerMemberName] string? cName = default)
    {
        var result = TryGetWrap(name, false);
        return IsErrStrict(result.Found, required, Settings.PropsRequired)
            ? throw ErrStrict(name, cName: cName)
            : result.Result.ConvertOrFallback(fallback);
    }

    [PrivateApi("Internal")]
    public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
    {
        path = path.KeepOrNew().Add($"PreWrap.{GetType()}", specs.Field);
        var result = TryGetWrap(specs.Field, wrapDefault: true).Result;
        return new(result: result, valueType: ValueTypesWithState.Dynamic, path: path) { Source = this, Name = "dynamic" };
    }

    #endregion

}