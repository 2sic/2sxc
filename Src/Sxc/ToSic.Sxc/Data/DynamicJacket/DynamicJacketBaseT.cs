using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json.Nodes;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Wrapper;
using static System.StringComparison;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Base class for DynamicJackets. You won't use this, just included in the docs. <br/>
    /// To check if something is an array or an object, use "IsArray"
    /// </summary>
    /// <typeparam name="T">The underlying type, either a JObject or a JToken</typeparam>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("just use the objects from AsDynamic, don't use this directly")]
    public abstract class DynamicJacketBase<T>: DynamicJacketBase, IReadOnlyList<object>, IWrapper<T>, IPropertyLookup, ISxcDynamicObject, ICanGetByName
    {
        [PrivateApi]
        protected T UnwrappedContents;
        public T GetContents() => UnwrappedContents;

        ///// <summary>
        ///// Check if it's an array.
        ///// </summary>
        ///// <returns>True if an array/list, false if an object.</returns>
        //public abstract bool IsList { get; }

        /// <summary>
        /// Primary constructor expecting a internal data object
        /// </summary>
        /// <param name="originalData">the original data we're wrapping</param>
        [PrivateApi]
        protected DynamicJacketBase(T originalData, CodeDataWrapper wrapper): base(wrapper)
        {
            UnwrappedContents = originalData;
        }

        ///// <summary>
        ///// Enable enumeration. When going through objects (properties) it will return the keys, not the values. <br/>
        ///// Use the [key] accessor to get the values as <see cref="DynamicJacketList"/> or <see cref="Data"/>
        ///// </summary>
        ///// <returns></returns>
        //[PrivateApi]
        //public abstract IEnumerator<object> GetEnumerator();


        /// <inheritdoc />
        [PrivateApi]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// If the object is just output, it should show the underlying json string
        /// </summary>
        /// <returns>the inner json string</returns>
        public override string ToString() => UnwrappedContents.ToString();

        ///// <inheritdoc />
        //public dynamic Get(string name) => FindValueOrNull(name, InvariantCultureIgnoreCase, null);

        /// <summary>
        /// Count array items or object properties
        /// </summary>
        public override int Count => UnwrappedContents is IList<JsonNode> jArray
            ? jArray.Count
            : UnwrappedContents is JsonObject jObject ? jObject.Count : 0;

        ///// <summary>
        ///// Not yet implemented accessor - must be implemented by the inheriting class.
        ///// </summary>
        ///// <param name="index"></param>
        ///// <returns>a <see cref="System.NotImplementedException"/></returns>
        //public abstract object this[int index] { get; }

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
        public new PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            path = path.KeepOrNew().Add("DynJacket", specs.Field);
            var result = FindValueOrNull(specs.Field, InvariantCultureIgnoreCase, specs.LogOrNull);
            return new PropReqResult(result: result, fieldType: Attributes.FieldIsDynamic, path: path) { Source = this, Name = "dynamic" };
        }

        //public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

        //protected abstract object FindValueOrNull(string name, StringComparison comparison, ILog parentLogOrNull);

    }
}
