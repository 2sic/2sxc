using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code
{
    public class CodeParameters : ICodeParameters
    {
        private readonly IDynamicCodeRoot _codeRoot;
        private readonly IDictionary<string, object> _paramsDictionary;

        public CodeParameters(IDictionary<string, object> paramsDictionary, IDynamicCodeRoot codeRoot)
        {
            _codeRoot = codeRoot;
            _paramsDictionary = paramsDictionary?.ToInvariant() ?? new Dictionary<string, object>();
        }

        #region Get and GetInternal

        public object Get(string name, string noParamOrder = Protector, bool required = false) 
            => GetInternal(name, noParamOrder, required);

        public T Get<T>(string name, string noParamOrder = Protector, T fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        private T GetInternal<T>(string name, string noParamOrder, T fallback, bool required, [CallerMemberName] string cName = default)
        {
            var found = GetInternal(name, noParamOrder, required, cName: cName);
            
            if (found == null) return fallback;

            // Already matching type OR Interface (because ConvertOrFallback can't handle interface)
            if (found is T typed) return typed;

            return typeof(T).IsInterface ? fallback : found.ConvertOrFallback(fallback);
        }

        private object GetInternal(string name, string noParamOrder = Protector, bool required = false, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, "required, fallback", cName);

            if (_paramsDictionary.TryGetValue(name, out var result))
                return result;
            if (!required)
                return null;

            throw new ArgumentException($"Tried to get CodeParameters {nameof(cName)}('{name}', ..., required=true)" +
                                        " but parameters not provided.", nameof(name));
        }

        public (T typed, object untyped, bool ok) GetInternalForInterface<T>(string name, string noParamOrder, T fallback, bool required = false,
            [CallerMemberName] string cName = default) where T : class
        {
            var maybe = GetInternal(name, noParamOrder, required, cName);
            if (maybe == null) return (fallback, null, true);
            if (maybe is T typed) return (typed, maybe, true);

            return (null, maybe, false);
        }


        #endregion

        public dynamic Dynamic(string name, string noParamOrder = Protector, object fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        public string String(string name, string noParamOrder = Protector, string fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        #region Numbers

        public int Int(string name, string noParamOrder = Protector, int fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        public float Float(string name, string noParamOrder = Protector, float fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        public double Double(string name, string noParamOrder = Protector, double fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        public decimal Decimal(string name, string noParamOrder = Protector, decimal fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        #endregion

        public Guid Guid(string name, string noParamOrder = Protector, Guid fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        public bool Bool(string name, string noParamOrder = Protector, bool fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        public DateTime DateTime(string name, string noParamOrder = Protector, DateTime fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        public IEntity Entity(string name, string noParamOrder = Protector, IEntity fallback = default, bool required = false)
        {
            var (typed, untyped, ok) = GetInternalForInterface(name, noParamOrder, fallback, required);
            // Try to convert, in case it's an IEntity or something; could also result in error
            return ok ? typed : _codeRoot.AsEntity(untyped);
        }

        #region Adam

        public ITypedFile File(string name, string noParamOrder = Protector, ITypedFile fallback = default, bool required = false)
        {
            var (typed, untyped, ok) = GetInternalForInterface(name, noParamOrder, fallback, required);
            if (ok) return typed;

            // Flatten list if necessary
            if (untyped is IEnumerable<ITypedFile> list) return list.First();
            return fallback;
        }

        public IEnumerable<ITypedFile> Files(string name, string noParamOrder = Protector, IEnumerable<ITypedFile> fallback = default, bool required = false)
        {
            return GetInternal(name, noParamOrder, fallback, required);
        }

        public ITypedFolder Folder(string name, string noParamOrder = Protector, ITypedFolder fallback = default, bool required = false) 
            => GetInternal(name, noParamOrder, fallback, required);

        public IEnumerable<ITypedFolder> Folders(string name, string noParamOrder = Protector, IEnumerable<ITypedFolder> fallback = default, bool required = false)
        {
            var (typed, untyped, ok) = GetInternalForInterface(name, noParamOrder, fallback, required);
            if (ok) return typed;

            // Wrap into list if necessary
            if (untyped is ITypedFolder item) return new List<ITypedFolder> {item};
            return fallback;
        }

        #endregion


        public ITypedItem Item(string name, string noParamOrder = Protector, ITypedItem fallback = default, bool required = false)
        {
            var (typed, untyped, ok) = GetInternalForInterface(name, noParamOrder, fallback, required);
            // Try to convert, in case it's an IEntity or something; could also result in error
            return ok ? typed : _codeRoot.AsTyped(untyped);
        }

        public IEnumerable<ITypedItem> Items(string name, string noParamOrder = Protector, IEnumerable<ITypedItem> fallback = default, bool required = false)
        {
            var (typed, untyped, ok) = GetInternalForInterface(name, noParamOrder, fallback, required);
            // Try to convert, in case it's an IEntity or something; could also result in error
            return ok ? typed : _codeRoot.AsTypedList(untyped);
        }
    }
}
