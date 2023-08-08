using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Wrapper
{
    // WIP
    // Inspired by https://stackoverflow.com/questions/46948289/how-do-you-convert-any-c-sharp-object-to-an-expandoobject
    // That was more complex with ability so set new values and switch between case-insensitive or not but that's not the purpose of this
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Will always return a value even if the property doesn't exist, in which case it resolves to null.
    /// </remarks>
    [JsonConverter(typeof(DynamicJsonConverter))]
    public partial class PreWrapObject: IWrapper<object>, IPropertyLookup, IHasJsonSource
    {
        #region Constructor / Setup

        public object GetContents() => UnwrappedObject;
        private readonly Dictionary<string, PropertyInfo> _ignoreCaseLookup;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="settings">
        /// Determines if properties which are objects should again be wrapped.
        /// When using this for DynamicModel it should be false, otherwise usually true.
        /// </param>
        [PrivateApi]
        internal PreWrapObject(object item, WrapperSettings settings, DynamicWrapperFactory wrapperFactory)
        {
            WrapperFactory = wrapperFactory;
            UnwrappedObject = item;
            _settings = settings;
            _ignoreCaseLookup = CreateDictionary(item);
        }

        protected readonly DynamicWrapperFactory WrapperFactory;
        protected readonly object UnwrappedObject;
        private readonly WrapperSettings _settings;

        private static Dictionary<string, PropertyInfo> CreateDictionary(object original)
        {
            var dic = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            if (original is null) return dic;
            var itemType = original.GetType();
            foreach (var propertyInfo in itemType.GetProperties())
                dic[propertyInfo.Name] = propertyInfo;
            return dic;
        }

        #endregion

        #region Keys

        public bool ContainsKey(string name) => _ignoreCaseLookup.ContainsKey(name);

        public IEnumerable<string> Keys(string noParamOrder, IEnumerable<string> only)
            => FilterKeysIfPossible(noParamOrder, only, _ignoreCaseLookup?.Keys);

        #endregion


        /// <inheritdoc />
        [PrivateApi("Internal")]
        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            path = path.KeepOrNew().Add("DynReadObj", specs.Field);
            var result = TryGet(specs.Field, true).Result;
            return new PropReqResult(result: result, fieldType: Attributes.FieldIsDynamic, path: path) { Source = this, Name = "dynamic" };
        }

        public (bool Found, object Raw, object Result) TryGet(string name, bool wrapDefault = true)
        {
            if (UnwrappedObject == null)
                return (false, null, null);

            if (!_ignoreCaseLookup.TryGetValue(name, out var lookup))
                return (false, null, null);

            var result = lookup.GetValue(UnwrappedObject);

            // Probably re-wrap for further dynamic navigation!
            return (true, result,
                _settings.WrapChildren && wrapDefault
                ? WrapperFactory.WrapIfPossible(result, _settings.WrapRealObjects, _settings)
                : result);
        }

        public TValue TryGet<TValue>(string name, string noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var (found, raw, _) = TryGet(name, false);
            return IsErrStrict(found, required, _settings.GetStrict)
                ? throw ErrStrict(name)
                : raw.ConvertOrFallback(fallback);
        }

        //public TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, [CallerMemberName] string cName = default)
        //{
        //    Protect(noParamOrder, nameof(fallback), methodName: cName);
        //    return TryGet(name, false).Result.ConvertOrFallback(fallback);
        //}



        public object JsonSource => UnwrappedObject;
    }
}