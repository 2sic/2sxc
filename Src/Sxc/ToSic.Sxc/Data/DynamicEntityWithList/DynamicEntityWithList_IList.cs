using System;
using System.Collections.Generic;

namespace ToSic.Sxc.Data
{
    // The IList interface is supported for backwards compatibility
    // but in reality, changes etc. are not allowed, it's read-only
    public partial class DynamicEntityWithList: IList<IDynamicEntity>
    {
        #region Implemented features as read-only List
        public bool IsReadOnly => true;
        public bool Contains(IDynamicEntity item) => ListHelper.DynEntities.Contains(item);

        public void CopyTo(IDynamicEntity[] array, int arrayIndex) => ListHelper.DynEntities.CopyTo(array, arrayIndex);

        public int IndexOf(IDynamicEntity item) => ListHelper.DynEntities.IndexOf(item);

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
