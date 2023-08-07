using System;
using System.Collections.Generic;
using System.Dynamic;
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
    public partial class DynamicReadObject: DynamicObject, IWrapper<object>, IPropertyLookup, IHasJsonSource, ICanGetByName
    {
        [PrivateApi]
        public object GetContents() => UnwrappedObject;
        private readonly Dictionary<string, PropertyInfo> _ignoreCaseLookup = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="wrapChildren">
        /// Determines if properties which are objects should again be wrapped.
        /// When using this for DynamicModel it should be false, otherwise usually true.
        /// </param>
        [PrivateApi]
        internal DynamicReadObject(object item, bool wrapChildren, bool wrapRealChildren, DynamicWrapperFactory wrapperFactory)
        {
            _wrapChildren = wrapChildren;
            _wrapRealChildren = wrapRealChildren;
            WrapperFactory = wrapperFactory;
            UnwrappedObject = item;
            if (item == null) return;
            
            var itemType = item.GetType();
            foreach (var propertyInfo in itemType.GetProperties()) 
                _ignoreCaseLookup[propertyInfo.Name] = propertyInfo;
        }
        private readonly bool _wrapChildren;
        private readonly bool _wrapRealChildren;
        protected readonly DynamicWrapperFactory WrapperFactory;
        protected readonly object UnwrappedObject;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = FindValueOrNull(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) 
            => throw new NotSupportedException($"Setting a value on {nameof(DynamicReadObject)} is not supported");


        /// <inheritdoc />
        [PrivateApi("Internal")]
        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            path = path.KeepOrNew().Add("DynReadObj", specs.Field);
            var result = FindValueOrNull(specs.Field);
            return new PropReqResult(result: result, fieldType: Attributes.FieldIsDynamic, path: path) { Source = this, Name = "dynamic" };
        }


        private object FindValueOrNull(string name)
        {
            if (UnwrappedObject == null)
                return null;

            if(!_ignoreCaseLookup.TryGetValue(name, out var lookup))
                return null;

            var result = lookup.GetValue(UnwrappedObject);

            // Probably re-wrap for further dynamic navigation!
            return _wrapChildren 
                ? WrapperFactory.WrapIfPossible(result, _wrapRealChildren, _wrapChildren, _wrapRealChildren) 
                : result;
        }


        object IHasJsonSource.JsonSource => UnwrappedObject;
        public dynamic Get(string name) => FindValueOrNull(name);
    }
}