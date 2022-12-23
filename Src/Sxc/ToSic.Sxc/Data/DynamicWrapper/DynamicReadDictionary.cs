using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
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
    /// Will always return true even if the property doesn't exist, in which case it resolves to null.
    /// </remarks>
    public class DynamicReadDictionary<TKey, TVal>: DynamicObject, IWrapper<IDictionary<TKey, TVal>>
    {
        protected readonly IDictionary<TKey, TVal> UnwrappedDictionary;
        [PrivateApi]
        public IDictionary<TKey, TVal> GetContents() => UnwrappedDictionary;
        private readonly Dictionary<string, object> _ignoreCaseLookup = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public DynamicReadDictionary(IDictionary<TKey, TVal> dictionary)
        {
            UnwrappedDictionary = dictionary;
            if (dictionary == null) return;

            foreach (var de in dictionary) 
                _ignoreCaseLookup[de.Key.ToString()] = de.Value;
        }
        
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // if nothing found, just return true/done
            if(!_ignoreCaseLookup.TryGetValue(binder.Name, out result))
                return true;

            // if result is an anonymous object, re-wrap again for consistency with other APIs
            if (result is null) return true;
            if (result.IsAnonymous())
                result = DynamicHelpers.WrapIfPossible(result, false, true, false);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) 
            => throw new NotSupportedException($"Setting a value on DynamicReadDictionary is not supported");
    }
}