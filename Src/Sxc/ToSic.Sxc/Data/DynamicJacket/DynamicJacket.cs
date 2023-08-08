using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using static System.StringComparison;
using ToSic.Sxc.Data.Typed;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Case insensitive dynamic read-object for JSON. <br/>
    /// Used in various cases where you start with JSON and want to
    /// provide the contents to custom code without having to mess with
    /// JS/C# code style differences. <br/>
    /// You will usually do things like `AsDynamic(jsonString).FirstName` etc.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("just use the objects from AsDynamic(...), don't use this directly")]
    [JsonConverter(typeof(DynamicJsonConverter))]
    public partial class DynamicJacket: DynamicJacketBase<JsonObject>, IPropertyLookup, IHasJsonSource
    {
        /// <inheritdoc />
        [PrivateApi]
        internal DynamicJacket(JsonObject originalData, DynamicWrapperFactory wrapperFactory) : base(originalData, wrapperFactory) { }

        [PrivateApi]
        protected override bool TypedHasImplementation(string name) 
            => !name.IsEmpty() && UnwrappedContents.Any(p => name.EqualsInsensitive(p.Key));

        [PrivateApi]
        protected override IEnumerable<string> TypedKeysImplementation(string noParamOrder, IEnumerable<string> only) 
            => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, UnwrappedContents?.Select(p => p.Key));

        /// <inheritdoc />
        public override bool IsList => false;



        /// <summary>
        /// Enable enumeration. Will return the keys, not the values. <br/>
        /// Use the [key] accessor to get the values as <see cref="DynamicJacket"/> or <see cref="DynamicJacketList"/>
        /// </summary>
        /// <returns>the string names of the keys</returns>
        [InternalApi_DoNotUse_MayChangeWithoutNotice]
        public override IEnumerator<object> GetEnumerator() => UnwrappedContents.Select(p => p.Key).GetEnumerator();


        /// <summary>
        /// Access the properties of this object.
        /// </summary>
        /// <remarks>
        /// Note that <strong>this</strong> accessor is case insensitive
        /// </remarks>
        /// <param name="key">the key, case-insensitive</param>
        /// <returns>A value (string, int etc.), <see cref="DynamicJacket"/> or <see cref="DynamicJacketList"/></returns>
        public object this[string key] 
            => FindValueOrNull(key, InvariantCultureIgnoreCase, null);

        /// <summary>
        /// Access the properties of this object.
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="caseSensitive">true if case-sensitive, false if not</param>
        /// <returns>A value (string, int etc.), <see cref="DynamicJacket"/> or <see cref="DynamicJacketList"/></returns>
        [PrivateApi("2023-08-07 2dm made privet now, before it was public. It's very exotic, so I don't think it's used anywhere, consider removing this later on")]
        public object this[string key, bool caseSensitive]
            => FindValueOrNull(key, caseSensitive 
                ? Ordinal
                : InvariantCultureIgnoreCase, null);


        #region Private TryGetMember

        /// <summary>
        /// Performs a case-insensitive value look-up
        /// </summary>
        /// <param name="binder">.net binder object</param>
        /// <param name="result">usually a <see cref="DynamicJacket"/>, <see cref="DynamicJacketList"/> or null</param>
        /// <returns>always returns true, to avoid errors</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = FindValueOrNull(binder.Name, InvariantCultureIgnoreCase, null);
            // always say it was found to prevent runtime errors
            return true;
        }

        protected override object FindValueOrNull(string name, StringComparison comparison, ILog parentLogOrNull)
        {
            if (UnwrappedContents == null || !UnwrappedContents.Any())
                return null;

            var found = UnwrappedContents.FirstOrDefault(
                    p => string.Equals(p.Key, name, comparison));

            return WrapperFactory.IfJsonGetValueOrJacket(found.IsNullOrDefault() ? null : found.Value);
        }

        #endregion

        /// <inheritdoc />
        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        public override object this[int index] => (_propertyArray ?? (_propertyArray = UnwrappedContents.Select(p => p.Value).ToArray()))[index];

        private JsonNode[] _propertyArray;

        /// <inheritdoc />
        object IHasJsonSource.JsonSource => UnwrappedContents;
    }
}
