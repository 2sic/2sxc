using System;
using System.Collections.Generic;

namespace ToSic.Sxc.Web.Url
{
    /// <summary>
    /// Helper to process url values - and keep or skip certain properties.
    /// Note that it is case-insensitive
    /// </summary>
    public class UrlValueFilter
    {
        /// <summary>
        /// Determine names of properties to preserve in the final parameters
        /// </summary>
        /// <param name="defaultSerialize"></param>
        /// <param name="opposite"></param>
        public UrlValueFilter(bool defaultSerialize, IEnumerable<string> opposite)
        {
            PropSerializeDefault = defaultSerialize;
            foreach (var sProp in opposite)
                PropSerializeMap[sProp] = !PropSerializeDefault;
        }

        /// <summary>
        /// Determine if not-found properties should be preserved or not - default is preserve, but init can reverse this
        /// </summary>
        internal bool PropSerializeDefault;
        internal Dictionary<string, bool> PropSerializeMap = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);


        public (bool Keep, string name, object Value) FilterValues(string name, object value)
        {
            return PropSerializeMap.TryGetValue(name, out var reallyUse)
                ? (reallyUse, name, value)
                : (PropSerializeDefault, name, value);
        }
    }
}
