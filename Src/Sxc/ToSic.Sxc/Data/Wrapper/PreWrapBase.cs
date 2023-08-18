using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Wrapper
{
    public abstract class PreWrapBase
    {

        public abstract WrapperSettings Settings { get; }

        #region Abstract things which must be implemented like TryGetWrap

        public abstract object JsonSource { get; }

        public abstract TryGetResult TryGetWrap(string name, bool wrapDefault = true);

        public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

        #endregion

        #region Abstract: Keys

        public abstract IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default);

        public abstract bool ContainsKey(string name);

        #endregion


        public object TryGetObject(string name, string noParamOrder, bool? required, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(required));
            var result = TryGetWrap(name, true);
            return IsErrStrict(result.Found, required, Settings.GetStrict)
                ? throw ErrStrict(name, cName: cName)
                : result.Result;
        }

        public TValue TryGetTyped<TValue>(string name, string noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var result = TryGetWrap(name, false);
            return IsErrStrict(result.Found, required, Settings.GetStrict)
                ? throw ErrStrict(name, cName: cName)
                : result.Result.ConvertOrFallback(fallback);
        }

        /// <inheritdoc />
        [PrivateApi("Internal")]
        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            path = path.KeepOrNew().Add($"PreWrap.{GetType()}", specs.Field);
            var result = TryGetWrap(specs.Field, true).Result;
            return new PropReqResult(result: result, fieldType: Attributes.FieldIsDynamic, path: path) { Source = this, Name = "dynamic" };
        }


    }
}
