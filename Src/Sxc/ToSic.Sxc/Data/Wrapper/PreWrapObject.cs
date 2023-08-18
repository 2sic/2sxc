using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
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
    public partial class PreWrapObject: PreWrapBase, IWrapper<object>, IPropertyLookup, IHasJsonSource, IPreWrap
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
        internal PreWrapObject(object item, WrapperSettings settings, CodeDataWrapper wrapper)
        {
            Wrapper = wrapper;
            UnwrappedObject = item;
            Settings = settings;
            _ignoreCaseLookup = CreateDictionary(item);
        }

        protected readonly CodeDataWrapper Wrapper;
        protected readonly object UnwrappedObject;
        public override WrapperSettings Settings { get; }

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

        public override bool ContainsKey(string name) => _ignoreCaseLookup.ContainsKey(name);

        public override IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default)
            => FilterKeysIfPossible(noParamOrder, only, _ignoreCaseLookup?.Keys);

        #endregion


        public override TryGetResult TryGetWrap(string name, bool wrapDefault = true)
        {
            if (UnwrappedObject == null)
                return new TryGetResult(false, null, null);

            if (!_ignoreCaseLookup.TryGetValue(name, out var lookup))
                return new TryGetResult(false, null, null);

            var result = lookup.GetValue(UnwrappedObject);

            // Probably re-wrap for further dynamic navigation!
            return new TryGetResult(true, result,
                Settings.WrapChildren && wrapDefault
                ? Wrapper.ChildNonJsonWrapIfPossible(result, Settings.WrapRealObjects, Settings)
                : result);
        }

        //public object TryGet(string name, string noParamOrder, bool? required, [CallerMemberName] string cName = default)
        //{
        //    Protect(noParamOrder, nameof(required));
        //    var result = TryGetWrap(name, true);
        //    return IsErrStrict(result.Found, required, Settings.GetStrict)
        //        ? throw ErrStrict(name, cName: cName)
        //        : result.Result;
        //}

        //public TValue TryGetTyped<TValue>(string name, string noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default)
        //{
        //    Protect(noParamOrder, nameof(fallback), methodName: cName);
        //    var result = TryGetWrap(name, false);
        //    return IsErrStrict(result.Found, required, Settings.GetStrict)
        //        ? throw ErrStrict(name, cName: cName)
        //        : result.Raw.ConvertOrFallback(fallback);
        //}

        public override object JsonSource => UnwrappedObject;
    }
}