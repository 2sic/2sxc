using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
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
    public partial class AnalyzeObject: IWrapper<object>, IPropertyLookup, IHasJsonSource //, ICanGetByName
    {
        //[PrivateApi]
        public object GetContents() => UnwrappedObject;
        [PrivateApi]
        private readonly Dictionary<string, PropertyInfo> _ignoreCaseLookup;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="wrapChildren">
        /// Determines if properties which are objects should again be wrapped.
        /// When using this for DynamicModel it should be false, otherwise usually true.
        /// </param>
        [PrivateApi]
        public AnalyzeObject(object item, bool wrapChildren, bool wrapRealChildren, DynamicWrapperFactory wrapperFactory)
        {
            _wrapChildren = wrapChildren;
            _wrapRealChildren = wrapRealChildren;
            WrapperFactory = wrapperFactory;
            UnwrappedObject = item;
            _ignoreCaseLookup = CreateDictionary(item);
        }
        private readonly bool _wrapChildren;
        private readonly bool _wrapRealChildren;
        protected readonly DynamicWrapperFactory WrapperFactory;
        protected readonly object UnwrappedObject;

        private static Dictionary<string, PropertyInfo> CreateDictionary(object original)
        {
            var dic = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            if (original is null) return dic;
            var itemType = original.GetType();
            foreach (var propertyInfo in itemType.GetProperties())
                dic[propertyInfo.Name] = propertyInfo;
            return dic;
        }


        /// <inheritdoc />
        [PrivateApi("Internal")]
        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            path = path.KeepOrNew().Add("DynReadObj", specs.Field);
            var result = TryGet(specs.Field).Result;
            return new PropReqResult(result: result, fieldType: Attributes.FieldIsDynamic, path: path) { Source = this, Name = "dynamic" };
        }

        public (bool Found, object Raw, object Result) TryGet(string name)
        {
            if (UnwrappedObject == null)
                return (false, null, null);

            if (!_ignoreCaseLookup.TryGetValue(name, out var lookup))
                return (false, null, null);

            var result = lookup.GetValue(UnwrappedObject);

            // Probably re-wrap for further dynamic navigation!
            return (true, result,
                _wrapChildren
                ? WrapperFactory.WrapIfPossible(result, _wrapRealChildren, _wrapChildren, _wrapRealChildren)
                : result);
        }


        object IHasJsonSource.JsonSource => UnwrappedObject;
        public dynamic Get(string name) => TryGet(name).Result;
    }
}