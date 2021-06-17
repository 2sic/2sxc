using System;
using System.Collections;
using System.Collections.Generic;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: IList<IDynamicEntity>
    {
        public IEnumerator<IDynamicEntity> GetEnumerator() => _ListHelper.DynEntities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Todo: first check if there is a property with this name
        public int Count => Get("Count") ?? _ListHelper.DynEntities.Count;

        public IDynamicEntity this[int index]
        {
            get => _ListHelper.DynEntities[index];
            // note: set must be defined for IList<IDynamicEntity>
            set => throw new NotImplementedException();
        }


        #region Implemented features as read-only List
        public bool IsReadOnly => true;
        public bool Contains(IDynamicEntity item) => _ListHelper.DynEntities.Contains(item);

        public void CopyTo(IDynamicEntity[] array, int arrayIndex) => _ListHelper.DynEntities.CopyTo(array, arrayIndex);

        public int IndexOf(IDynamicEntity item) => _ListHelper.DynEntities.IndexOf(item);

        #endregion

        #region Not implemented IList interfaces
        public void Add(IDynamicEntity item) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        public bool Remove(IDynamicEntity item) => throw new NotImplementedException();

        public void Insert(int index, IDynamicEntity item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();
        #endregion

    }
}
