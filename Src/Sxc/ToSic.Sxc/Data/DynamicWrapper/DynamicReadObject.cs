using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using Newtonsoft.Json;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Data
{
    // WIP
    // Inspired by https://stackoverflow.com/questions/46948289/how-do-you-convert-any-c-sharp-object-to-an-expandoobject
    // That was more complex with ability so set new values and switch between case-insensitive or not but that's not the purpose of this
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Will always return true even if the property doesn't exist, in which case it resolves to null.
    /// </remarks>
    [JsonConverter(typeof(DynamicJsonConverter))]
    public partial class DynamicReadObject: DynamicObject, IWrapper<object>, IPropertyLookup, IHasJsonSource
    {
        public object UnwrappedContents { get; }
        private readonly Dictionary<string, PropertyInfo> _ignoreCaseLookup = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reWrapObjects">Determines if properties which are objects should again be wrapped. When using this for DynamicModel it should be false, otherwise usually true.</param>
        [PrivateApi]
        public DynamicReadObject(object item, bool reWrapObjects)
        {
            _reWrapObjects = reWrapObjects;
            UnwrappedContents = item;
            if (item == null) return;
            
            var itemType = item.GetType();
            foreach (var propertyInfo in itemType.GetProperties()) _ignoreCaseLookup[propertyInfo.Name] = propertyInfo;
        }
        private readonly bool _reWrapObjects;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = FindValueOrNull(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) 
            => throw new NotSupportedException($"Setting a value on {nameof(DynamicReadObject)} is not supported");

        [PrivateApi]
        public PropertyRequest FindPropertyInternal(string field, string[] languages, ILog parentLogOrNull)
        {
            var result = FindValueOrNull(field);
            return new PropertyRequest { Result = result, FieldType = Attributes.FieldIsDynamic, Source = this, Name = "dynamic" };
        }


        private object FindValueOrNull(string name)
        {
            if (UnwrappedContents == null)
                return null;

            if(!_ignoreCaseLookup.TryGetValue(name, out var lookup))
                return null;

            var result = lookup.GetValue(UnwrappedContents);

            // Probably re-wrap for further dynamic navigation!
            return _reWrapObjects ? DynamicHelpers.WrapIfPossible(result) : result;
        }


        object IHasJsonSource.JsonSource => UnwrappedContents;
    }
}