using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using ToSic.Eav.Data;

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
    public class DynamicReadObject: DynamicObject, IWrapper<object>
    {
        public object UnwrappedContents { get; }
        private readonly Dictionary<string, PropertyInfo> _ignoreCaseLookup = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

        public DynamicReadObject(object item)
        {
            UnwrappedContents = item;
            if (item == null) return;
            
            var itemType = item.GetType();
            foreach (var propertyInfo in itemType.GetProperties()) _ignoreCaseLookup[propertyInfo.Name] = propertyInfo;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            _ignoreCaseLookup.TryGetValue(binder.Name, out var lookup);

            if (lookup == null) return true;
            result = lookup.GetValue(UnwrappedContents);
            return true;

        }

        public override bool TrySetMember(SetMemberBinder binder, object value) 
            => throw new NotImplementedException($"Setting a value on {nameof(DynamicReadObject)} is not supported");
    }
}