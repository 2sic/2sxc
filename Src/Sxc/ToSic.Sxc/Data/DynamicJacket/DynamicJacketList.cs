using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a DynamicJacket for JSON arrays. You can enumerate through it. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("just use the objects from AsDynamic, don't use this directly")]
    public class DynamicJacketList : DynamicJacketBase<JsonArray>, IReadOnlyList<object>
    {
        /// <inheritdoc />
        public DynamicJacketList(JsonArray originalData, DynamicWrapperFactory wrapperFactory) :base(originalData, wrapperFactory) { }

        protected override bool TypedHasImplementation(string name)
        {
            if (name.IsEmpty() || UnwrappedContents == null) return false;
            var index = name.ConvertOrFallback<int>(fallback: -1, numeric: true);
            if (index == -1) return false;
            return UnwrappedContents.Count <= index;
        }

        /// <inheritdoc />
        public override bool IsList => true;

        [PrivateApi]
        public override IEnumerator<object> GetEnumerator() 
            => UnwrappedContents.Select(WrapperFactory.WrapIfJObjectUnwrapIfJValue).GetEnumerator();

        /// <summary>
        /// Access the items in this object - but only if the underlying object is an array. 
        /// </summary>
        /// <param name="index">array index</param>
        /// <returns>the item or an error if not found</returns>
        public override object this[int index] => WrapperFactory.WrapIfJObjectUnwrapIfJValue(UnwrappedContents[index]);

        [PrivateApi("internal")]
        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) 
            => new List<PropertyDumpItem> { new PropertyDumpItem { Path = "Not supported on DynamicJacket" } };

        /// <summary>
        /// On a dynamic Jacket List where is no reasonable convention how to find something by name
        /// since it's not clear which property would be the name-giving property. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="comparison"></param>
        /// <param name="parentLogOrNull"></param>
        /// <returns></returns>
        protected override object FindValueOrNull(string name, StringComparison comparison, ILog parentLogOrNull)
        {
            if (UnwrappedContents == null || !UnwrappedContents.Any())
                return null;

            var found = UnwrappedContents.FirstOrDefault(p =>
                {
                    if (!(p is JsonObject pJObject))
                        return false;

                    if (HasPropertyWithValue(pJObject, "Name", name, comparison))
                        return true;

                    if (HasPropertyWithValue(pJObject, "Title", name, comparison))
                        return true;

                    return false;
                });

            return WrapperFactory.WrapIfJObjectUnwrapIfJValue(found);
        }

        private bool HasPropertyWithValue(JsonObject obj, string propertyName, string value, StringComparison comparison)
        {
            if (obj.TryGetPropertyValue(propertyName, out var propertyValue) && propertyValue is JsonValue jValResult && jValResult.TryGetValue<string>(out var stringValue))
                return string.Equals(stringValue, value, comparison);

            return false;

        }

    }
}
