using System;
using System.Dynamic;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Data
{
    public static class DynamicHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="wrapRealObjects">if true and the contents isn't already a dynamic object, it will also wrap real objects; otherwise only anonymous</param>
        /// <param name="wrapChildren"></param>
        /// <param name="wrapRealChildren"></param>
        /// <returns></returns>
        [PrivateApi]
        internal static object WrapIfPossible(object value, bool wrapRealObjects, bool wrapChildren, bool wrapRealChildren)
        {
            // If null or simple value, use that
            if (value is null) return null;

            if (value is string || value.GetType().IsValueType) return value;

            // Guids & DateTimes are objects, but very simple, and should be returned for normal use
            if (value is Guid || value is DateTime) return value;

            // Check if the result is a JSON object which should support navigation again
            var result = DynamicJacket.WrapIfJObjectUnwrapIfJValue(value);

            // Check if the result already supports navigation... - which is the case if it's a DynamicJacket now
            switch (result)
            {
                case IPropertyLookup _:
                case ISxcDynamicObject _:
                case DynamicObject _:
                    return result;
            }

            // if (result is IDictionary dicResult) return DynamicReadDictionary(dicResult);

            // Simple arrays don't benefit from re-wrapping. 
            // If the calling code needs to do something with them, it should iterate it and then rewrap with AsDynamic
            if (result.GetType().IsArray) return result;

            // Otherwise it's a complex object, which should be re-wrapped for navigation
            var wrap = wrapRealObjects || value.IsAnonymous();
            // var dontWrap = onlyRewrapDynamic && value.IsAnonymous();
            return wrap ? new DynamicReadObject(value, wrapChildren, wrapRealChildren): value;
        }
    }
}
