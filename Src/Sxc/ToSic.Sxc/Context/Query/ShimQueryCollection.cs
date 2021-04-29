using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ToSic.Sxc.Context.Query
{
    internal class ShimQueryCollection: IQueryCollection
    {
        public ShimQueryCollection(List<KeyValuePair<string, string>> originals)
        {

        }
        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            throw new NotImplementedException();
        }

        public int Count { get; }
        public ICollection<string> Keys { get; }

        public StringValues this[string key] => throw new NotImplementedException();
    }
}
