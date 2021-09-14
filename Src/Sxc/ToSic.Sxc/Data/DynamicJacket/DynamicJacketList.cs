using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a DynamicJacket for JSON arrays. You can enumerate through it. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("just use the objects from AsDynamic, don't use this directly")]
    public class DynamicJacketList : DynamicJacketBase<JArray>, IReadOnlyList<object>
    {
        /// <inheritdoc />
        public DynamicJacketList(JArray originalData) :base(originalData) { }

        /// <inheritdoc />
        public override bool IsList => true;

        /// <inheritdoc />
        public override IEnumerator<object> GetEnumerator() 
            => UnwrappedContents.Select(DynamicJacket.WrapIfJObjectUnwrapIfJValue).GetEnumerator();

        /// <summary>
        /// Access the items in this object - but only if the underlying object is an array. 
        /// </summary>
        /// <param name="index">array index</param>
        /// <returns>the item or an error if not found</returns>
        public override object this[int index] => DynamicJacket.WrapIfJObjectUnwrapIfJValue(UnwrappedContents[index]);

        [PrivateApi("internal")]
        public override List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull)
        {
            return new List<PropertyDumpItem> { new PropertyDumpItem { Path = "Not supported on DynamicJacket" } };
        }

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
            if (UnwrappedContents == null || !UnwrappedContents.HasValues)
                return null;

            var found = UnwrappedContents.FirstOrDefault(p =>
                {
                    if (!(p is JObject pJObject))
                        return false;

                    if (HasPropertyWithValue(pJObject, "Name", name, comparison))
                        return true;

                    if (HasPropertyWithValue(pJObject, "Title", name, comparison))
                        return true;

                    return false;
                });

            return DynamicJacket.WrapIfJObjectUnwrapIfJValue(found);
        }

        private bool HasPropertyWithValue(JObject obj, string propertyName, string value, StringComparison comparison)
        {
            if (obj.TryGetValue(propertyName, out var nameResult) && nameResult is JValue jValResult && jValResult.Type == JTokenType.String)
                return string.Equals(jValResult.Value as string, value, comparison);

            return false;

        }

    }
}
