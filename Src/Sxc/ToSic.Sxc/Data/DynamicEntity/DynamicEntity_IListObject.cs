using System;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Data
{
    // Backward compatible enumeration interface for people using IList<dynamic>
    
    public partial class DynamicEntity: IList<object>
    {
        #region Implemented features as read-only List

        IEnumerator<object> IEnumerable<object>.GetEnumerator() => _ListHelper.DynEntities.GetEnumerator();
        public bool Contains(object item) => _ListHelper.DynEntities.Contains(item);

        public int IndexOf(object item) => _ListHelper.DynEntities.IndexOf(item as IDynamicEntity);
        public void CopyTo(object[] array, int arrayIndex)
        {
            var target = new IDynamicEntity[_ListHelper.DynEntities.Count];
            _ListHelper.DynEntities.CopyTo(target, arrayIndex);
            target.CopyTo(array, arrayIndex);
        }

        #endregion

        #region Not implemented IList interfaces

        public void Add(object item) => throw new NotImplementedException();


        public bool Remove(object item) => throw new NotImplementedException();


        public void Insert(int index, object item) => throw new NotImplementedException();

        #endregion

        object IList<object>.this[int index]
        {
            get => (this as IList<IDynamicEntity>)[index];
            set => throw new NotImplementedException();
        }

    }
}
