using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json.Nodes;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Base class for DynamicJackets. You won't use this, just included in the docs. <br/>
    /// To check if something is an array or an object, use "IsArray"
    /// </summary>
    /// <typeparam name="T">The underlying type, either a JObject or a JToken</typeparam>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("just use the objects from AsDynamic, don't use this directly")]
    public abstract class DynamicJacketBase<T>: DynamicObject, IReadOnlyList<object>, IWrapper<T>, IPropertyLookup, ISxcDynamicObject, ICanGetByName
    {
        // 2022-12-23 2dm Removed - use GetContents
        ///// <summary>
        ///// The underlying data, in case it's needed for various internal operations.
        ///// </summary>
        //[PrivateApi]
        //public T UnwrappedContents => _contents;

        [PrivateApi]
        protected T UnwrappedContents;
        public T GetContents() => UnwrappedContents;

        /// <summary>
        /// Check if it's an array.
        /// </summary>
        /// <returns>True if an array/list, false if an object.</returns>
        public abstract bool IsList { get; }

        /// <summary>
        /// Primary constructor expecting a internal data object
        /// </summary>
        /// <param name="originalData">the original data we're wrapping</param>
        [PrivateApi]
        protected DynamicJacketBase(T originalData) => UnwrappedContents = originalData;

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

        /// <summary>
        /// If the object is just output, it should show the underlying json string
        /// </summary>
        /// <returns>the inner json string</returns>
        public override string ToString() => UnwrappedContents.ToString();

        /// <inheritdoc />
        public dynamic Get(string name) => FindValueOrNull(name, StringComparison.InvariantCultureIgnoreCase, null);

        /// <inheritdoc />
        public int Count => UnwrappedContents is IList<JsonNode> jArray
            ? jArray.Count
            : UnwrappedContents is JsonObject jObject ? jObject.Count : 0;

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
            var result = FindValueOrNull(specs.Field, StringComparison.InvariantCultureIgnoreCase, specs.LogOrNull);
            return new PropReqResult(result, path) { FieldType = Attributes.FieldIsDynamic, Source = this, Name = "dynamic" };
        }

        public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

        protected abstract object FindValueOrNull(string name, StringComparison comparison, ILog parentLogOrNull);

    }
}
