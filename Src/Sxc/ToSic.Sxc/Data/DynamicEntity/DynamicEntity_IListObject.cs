namespace ToSic.Sxc.Data;
// Backward compatible enumeration interface for people casting as IList<dynamic>

public partial class DynamicEntity: IList<object>
{
    #region IList explicit implementations (were already explicit before v16.03

    [PrivateApi("would confuse")]
    int ICollection<object>.Count => ListHelper.DynEntities.Count;

    [PrivateApi("Hide, will only confuse")]
    IEnumerator<object> IEnumerable<object>.GetEnumerator() => ListHelper.DynEntities.GetEnumerator();

    #endregion

    #region IList Things which were implicit before v16.03 and made explicit, in hope it was never used

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    bool ICollection<object>.Contains(object item) => ListHelper.DynEntities.Contains(item);

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    int IList<object>.IndexOf(object item) => ListHelper.DynEntities.IndexOf(item as IDynamicEntity);

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void ICollection<object>.CopyTo(object[] array, int arrayIndex)
    {
        var target = new IDynamicEntity[ListHelper.DynEntities.Count];
        ListHelper.DynEntities.CopyTo(target, arrayIndex);
        target.CopyTo(array, arrayIndex);
    }

    #endregion

    #region Not implemented IList interfaces - changed to explicit implementation v16.03

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void ICollection<object>.Add(object item) => throw new NotSupportedException();


    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    bool ICollection<object>.Remove(object item) => throw new NotSupportedException();


    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void IList<object>.Insert(int index, object item) => throw new NotSupportedException();

    #endregion

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    object IList<object>.this[int index]
    {
        get => (this as IList<IDynamicEntity>)[index];
        set => throw new NotSupportedException();
    }

}