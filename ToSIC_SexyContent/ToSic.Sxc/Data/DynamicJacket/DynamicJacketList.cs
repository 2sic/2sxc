using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a DynamicJacket for JSON arrays. You can enumerate through it. 
    /// </summary>
    [PublicApi]
    public class DynamicJacketList : DynamicJacketBase<JArray>, IReadOnlyList<object>
    {
        /// <inheritdoc />
        public DynamicJacketList(JArray originalData) :base(originalData) { }

        /// <inheritdoc />
        public override bool IsList => true;

        /// <inheritdoc />
        public override IEnumerator<object> GetEnumerator() 
            => UnwrappedContents.Select(DynamicJacket.WrapOrUnwrap).GetEnumerator();

        /// <summary>
        /// Access the items in this object - but only if the underlying object is an array. 
        /// </summary>
        /// <param name="index">array index</param>
        /// <returns>the item or an error if not found</returns>
        public override object this[int index] => DynamicJacket.WrapOrUnwrap(UnwrappedContents[index]);

    }
}
