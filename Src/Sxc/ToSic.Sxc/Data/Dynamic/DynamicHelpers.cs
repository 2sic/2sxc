using System;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    public static class DynamicHelpers
    {
        [PrivateApi]
        internal static object WrapIfPossible(object value)
        {
            // If null or simple value, use that
            if (value is null) return null;

            if (value is string || value.GetType().IsValueType) return value;

            // Guids & DateTimes are objects, but very simple, and should be returned for normal use
            if (value is Guid || value is DateTime) return value;

            // Check if the result is a JSON object which should support navigation again
            var result = DynamicJacket.WrapIfJObjectUnwrapIfJValue(value);

            // Check if the result already supports navigation... - which is the case if it's a DynamicJacket now
            if (result is IPropertyLookup) return result;

            if (result is ISxcDynamicObject) return result;

            if (result is DynamicObject) return result;

            // if (result is IDictionary dicResult) return DynamicReadDictionary(dicResult);

            // Simple arrays don't benefit from re-wrapping. 
            // If the calling code needs to do something with them, it should iterate it and then rewrap with AsDynamic
            if (result.GetType().IsArray) return result;

            // Otherwise it's a complex object, which should be re-wrapped for navigation
            return new DynamicReadObject(value, true);
        }
    }
}
