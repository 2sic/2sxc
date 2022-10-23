using System;
using System.Collections;
using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: IList<IDynamicEntity>
    {
        /// <inheritdoc />
        [PrivateApi("Hide, will only confuse")]
        public IEnumerator<IDynamicEntity> GetEnumerator() => _ListHelper.DynEntities.GetEnumerator();

        /// <inheritdoc />
        [PrivateApi("Hide, will only confuse")]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Shows how many Entities are available if you use foreach. Will usually return an `int`.
        /// </summary>
        /// <remarks>
        /// If the Entity contains a field `Count`, that will be returned instead.
        /// </remarks>
        /// <returns>Usually an `int` but if the Entity contains such a property, then it has the type of that property. </returns>
        public object Count => Get("Count") ?? _ListHelper.DynEntities.Count;
        
        [PrivateApi]
        int ICollection<IDynamicEntity>.Count => _ListHelper.DynEntities.Count;

        [PrivateApi("would only confuse users, not useful in Razor / DynamicCode")]
        public IDynamicEntity this[int index]
        {
            get => _ListHelper.DynEntities[index];
            // note: set must be defined for IList<IDynamicEntity>
            set => throw new NotSupportedException();
        }


        #region Implemented features as read-only List
        [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
        public bool IsReadOnly => true;


        [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
        public bool Contains(IDynamicEntity item) => _ListHelper.DynEntities.Contains(item);

        [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
        public void CopyTo(IDynamicEntity[] array, int arrayIndex) => _ListHelper.DynEntities.CopyTo(array, arrayIndex);

        [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
        public int IndexOf(IDynamicEntity item) => _ListHelper.DynEntities.IndexOf(item);

        #endregion

        #region Not implemented IList interfaces
        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public void Add(IDynamicEntity item) => throw new NotSupportedException();

        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public void Clear() => throw new NotSupportedException();

        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public bool Remove(IDynamicEntity item) => throw new NotSupportedException();

        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public void Insert(int index, IDynamicEntity item) => throw new NotSupportedException();

        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public void RemoveAt(int index) => throw new NotSupportedException();
        #endregion

    }
}
