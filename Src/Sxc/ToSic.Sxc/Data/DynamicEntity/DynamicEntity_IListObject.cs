using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    // Backward compatible enumeration interface for people using IList<dynamic>
    
    public partial class DynamicEntity: IList<object>
    {
        [PrivateApi("would confuse")]
        int ICollection<object>.Count => _ListHelper.DynEntities.Count;
        #region Implemented features as read-only List

        [PrivateApi("Hide, will only confuse")]
        IEnumerator<object> IEnumerable<object>.GetEnumerator() => _ListHelper.DynEntities.GetEnumerator();
        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public bool Contains(object item) => _ListHelper.DynEntities.Contains(item);

        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public int IndexOf(object item) => _ListHelper.DynEntities.IndexOf(item as IDynamicEntity);
        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public void CopyTo(object[] array, int arrayIndex)
        {
            var target = new IDynamicEntity[_ListHelper.DynEntities.Count];
            _ListHelper.DynEntities.CopyTo(target, arrayIndex);
            target.CopyTo(array, arrayIndex);
        }

        #endregion

        #region Not implemented IList interfaces

        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public void Add(object item) => throw new NotSupportedException();


        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public bool Remove(object item) => throw new NotSupportedException();


        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        public void Insert(int index, object item) => throw new NotSupportedException();

        #endregion

        [PrivateApi("Hide as it won't work")]
        [Obsolete("Don't use this, it's not supported")]
        object IList<object>.this[int index]
        {
            get => (this as IList<IDynamicEntity>)[index];
            set => throw new NotSupportedException();
        }

    }
}
