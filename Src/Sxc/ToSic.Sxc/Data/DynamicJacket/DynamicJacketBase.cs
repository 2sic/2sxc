using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using static System.StringComparison;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Base class for DynamicJackets. You won't use this, just included in the docs. <br/>
    /// To check if something is an array or an object, use "IsArray"
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("just use the objects from AsDynamic, don't use this directly")]
    public abstract partial class DynamicJacketBase: DynamicObject, IReadOnlyList<object>, IPropertyLookup, ISxcDynamicObject, ICanGetByName
    {

        /// <summary>
        /// Check if it's an array.
        /// </summary>
        /// <returns>True if an array/list, false if an object.</returns>
        public abstract bool IsList { get; }

        /// <summary>
        /// Enable enumeration. When going through objects (properties) it will return the keys, not the values. <br/>
        /// Use the [key] accessor to get the values as <see cref="DynamicJacketList"/> or <see cref="Data"/>
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        public abstract IEnumerator<object> GetEnumerator();


        /// <inheritdoc />
        [PrivateApi]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public dynamic Get(string name) => FindValueOrNull(name, InvariantCultureIgnoreCase, null);

        /// <summary>
        /// Count array items or object properties
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Not yet implemented accessor - must be implemented by the inheriting class.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>a <see cref="System.NotImplementedException"/></returns>
        public abstract object this[int index] { get; }

        /// <summary>
        /// Fake property binder - just ensure that simple properties don't cause errors. <br/>
        /// Must be overriden in inheriting objects
        /// like <see cref="DynamicJacketList"/>, <see cref="DynamicJacket"/>
        /// </summary>
        /// <param name="binder">.net binder object</param>
        /// <param name="result">always null, unless overriden</param>
        /// <returns>always returns true, to avoid errors</returns>
        [PrivateApi]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            return true;
        }

        /// <inheritdoc />
        [PrivateApi("Internal")]
        public PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            path = path.KeepOrNew().Add("DynJacket", specs.Field);
            var result = FindValueOrNull(specs.Field, InvariantCultureIgnoreCase, specs.LogOrNull);
            return new PropReqResult(result, path) { FieldType = Attributes.FieldIsDynamic, Source = this, Name = "dynamic" };
        }

        public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

        protected abstract object FindValueOrNull(string name, StringComparison comparison, ILog parentLogOrNull);

    }
}
