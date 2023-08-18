using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Wrapper;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    internal abstract class PreWrapJsonBase: IPreWrap, IPropertyLookup //, IHasKeys
    {
        internal PreWrapJsonBase(CodeJsonWrapper wrapper, WrapperSettings settings)
        {
            Wrapper = wrapper;
            Settings = settings;
        }

        public readonly CodeJsonWrapper Wrapper;

        public WrapperSettings Settings { get; }

        #region Abstract

        //public abstract object Get(string name);
        public abstract object JsonSource { get; }

        #endregion

        #region TryGetWrap

        public abstract TryGetResult TryGetWrap(string name, bool wrapDefault = true);
        #endregion

        #region IPropertyLookup Interface

        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            path = path.KeepOrNew().Add("DynJacket", specs.Field);
            var result = TryGetWrap(specs.Field).Result;
            return new PropReqResult(result: result, fieldType: Attributes.FieldIsDynamic, path: path) { Source = this, Name = "dynamic" };
        }

        public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

        #endregion

        #region Get for Typed

        [PrivateApi]
        public TValue TryGetTyped<TValue>(string name, string noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var result = TryGetWrap(name).Result;
            return result.ConvertOrFallback(fallback);
        }

        #endregion

        #region Keys (abstract)

        public abstract IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default);

        public abstract bool ContainsKey(string name);

        #endregion

        //public bool IsNotEmpty(string name, string noParamOrder = Protector)
        //    => HasKeysHelper.IsNotEmpty(Get(name), null);

        //public bool IsEmpty(string name, string noParamOrder = Protector) 
        //    => HasKeysHelper.IsEmpty(Get(name), null);
    }
}
