using System.Collections;

namespace ToSic.Sxc.Data;

public partial class DynamicEntity: IList<IDynamicEntity>
{
    [PrivateApi("Hide, will only confuse")]
    public IEnumerator<IDynamicEntity> GetEnumerator() => ListHelper.DynEntities.GetEnumerator();

    [PrivateApi("Hide, will only confuse")]
    IEnumerator IEnumerable.GetEnumerator() => ListHelper.DynEntities.GetEnumerator();

    /// <summary>
    /// Shows how many Entities are available if you use foreach. Will usually return an `int`.
    /// </summary>
    /// <remarks>
    /// If the Entity contains a field `Count`, that will be returned instead.
    /// </remarks>
    /// <returns>Usually an `int` but if the Entity contains such a property, then it has the type of that property. </returns>
    public object Count => Get("Count") ?? ListHelper.DynEntities.Count;
        
    [PrivateApi]
    int ICollection<IDynamicEntity>.Count => ListHelper.DynEntities.Count;

    [PrivateApi("would only confuse users, not useful in Razor / DynamicCode")]
    public IDynamicEntity this[int index]
    {
        get => ListHelper.DynEntities[index];
        // note: set must be defined for IList<IDynamicEntity>
        set => throw new NotSupportedException();
    }


    #region Implemented features as read-only List

    [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
    bool ICollection<object>.IsReadOnly => true;

    [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
    bool ICollection<IDynamicEntity>.IsReadOnly => true;


    [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
    public bool Contains(IDynamicEntity item) => ListHelper.DynEntities.Contains(item);

    [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
    public void CopyTo(IDynamicEntity[] array, int arrayIndex) => ListHelper.DynEntities.CopyTo(array, arrayIndex);

    [PrivateApi("Hide, necessary because of an Interface but would only confuse users")]
    public int IndexOf(IDynamicEntity item) => ListHelper.DynEntities.IndexOf(item);

    #endregion

    #region Not implemented ICollection<T> & IList<T> interfaces

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void ICollection<IDynamicEntity>.Add(IDynamicEntity item) => throw new NotSupportedException();

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void ICollection<object>.Clear() => throw new NotSupportedException();

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void ICollection<IDynamicEntity>.Clear() => throw new NotSupportedException();

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    bool ICollection<IDynamicEntity>.Remove(IDynamicEntity item) => throw new NotSupportedException();

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void IList<IDynamicEntity>.Insert(int index, IDynamicEntity item) => throw new NotSupportedException();

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void IList<object>.RemoveAt(int index) => throw new NotSupportedException();

    [PrivateApi("Hide as it won't work")]
    [Obsolete("Don't use this, it's not supported")]
    void IList<IDynamicEntity>.RemoveAt(int index) => throw new NotSupportedException();

    #endregion

}