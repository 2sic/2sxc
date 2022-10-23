using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text.Json.Serialization;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Logging;
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
        public object UnwrappedContents => _contents;
        public object GetContents() => _contents;
        private readonly Dictionary<string, PropertyInfo> _ignoreCaseLookup = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="wrapChildren">Determines if properties which are objects should again be wrapped. When using this for DynamicModel it should be false, otherwise usually true.</param>
        [PrivateApi]
        public DynamicReadObject(object item, bool wrapChildren, bool wrapRealChildren)
        {
            _wrapChildren = wrapChildren;
            _wrapRealChildren = wrapRealChildren;
            _contents = item;
            if (item == null) return;
            
            var itemType = item.GetType();
            foreach (var propertyInfo in itemType.GetProperties()) _ignoreCaseLookup[propertyInfo.Name] = propertyInfo;
        }
        private readonly bool _wrapChildren;
        private readonly bool _wrapRealChildren;
        private object _contents;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = FindValueOrNull(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) 
            => throw new NotSupportedException($"Setting a value on {nameof(DynamicReadObject)} is not supported");

        [PrivateApi]
        public PropertyRequest FindPropertyInternal(string field, string[] languages, ILog parentLogOrNull, PropertyLookupPath path)
        {
            path = path.KeepOrNew().Add("DynReadObj", field);
            var result = FindValueOrNull(field);
            return new PropertyRequest { Result = result, FieldType = Attributes.FieldIsDynamic, Source = this, Name = "dynamic", Path = path };
        }


        private object FindValueOrNull(string name)
        {
            if (_contents == null)
                return null;

            if(!_ignoreCaseLookup.TryGetValue(name, out var lookup))
                return null;

            var result = lookup.GetValue(_contents);

            // Probably re-wrap for further dynamic navigation!
            return _wrapChildren 
                ? DynamicHelpers.WrapIfPossible(result, _wrapRealChildren, _wrapChildren, _wrapRealChildren) 
                : result;
        }


        object IHasJsonSource.JsonSource => _contents;
        public dynamic Get(string name) => FindValueOrNull(name);
    }
}