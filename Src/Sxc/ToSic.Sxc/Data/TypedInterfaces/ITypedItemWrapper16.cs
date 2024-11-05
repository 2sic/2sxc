namespace ToSic.Sxc.Data;

/// <summary>
/// WIP v17.02+ - not sure if we will ever publish this...
/// </summary>
[PrivateApi("WIP, don't publish yet")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ITypedItemWrapper16
{
    // TODO: consider alternatives, like using an Attribute on the class!
    // that could be optional

    /// <summary>
    /// Internal functionality, so the object can declare what content Type it's for.
    ///
    /// By default, it will use the content-type name, but providing this property would allow
    /// other classes (with different names) to provide the proper name.
    ///
    /// ATM it's only used in App.Data.GetOne{T} and App.Data.GetAll{T}
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    string ForContentType { get; }

    /// <summary>
    /// Add the data to use for the wrapper.
    /// It can't be done in the constructor, because the object needs to have an empty (or future: maybe DI-compatible)
    /// constructor. 
    /// </summary>
    /// <param name="baseItem"></param>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    void Setup(ITypedItem baseItem);
}