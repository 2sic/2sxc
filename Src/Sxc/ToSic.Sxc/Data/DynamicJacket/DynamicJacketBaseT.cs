using System.Collections;
using ToSic.Lib.Data;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data;

/// <summary>
/// Base class for DynamicJackets. You won't use this, just included in the docs. <br/>
/// To check if something is an array or an object, use "IsArray"
/// </summary>
/// <typeparam name="T">The underlying type, either a JObject or a JToken</typeparam>
[PrivateApi("was Internal-API till v17 - just use the objects from AsDynamic, don't use this directly")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class DynamicJacketBase<T>: DynamicJacketBase, IReadOnlyList<object>, IWrapper<T>, ISxcDynamicObject, ICanGetByName
{
    [PrivateApi]
    protected T UnwrappedContents;
    public T GetContents() => UnwrappedContents;

    /// <summary>
    /// Primary constructor expecting a internal data object
    /// </summary>
    /// <param name="wrapper">Wrapper helper</param>
    /// <param name="originalData">the original data we're wrapping</param>
    [PrivateApi]
    internal DynamicJacketBase(CodeJsonWrapper wrapper, T originalData): base(wrapper)
    {
        UnwrappedContents = originalData;
    }

    ///// <summary>
    ///// Enable enumeration. When going through objects (properties) it will return the keys, not the values. <br/>
    ///// Use the [key] accessor to get the values as <see cref="DynamicJacketList"/> or <see cref="Data"/>
    ///// </summary>
    ///// <returns></returns>
    //[PrivateApi]
    //public abstract IEnumerator<object> GetEnumerator();


    /// <inheritdoc />
    [PrivateApi]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// If the object is just output, it should show the underlying json string
    /// </summary>
    /// <returns>the inner json string</returns>
    public override string ToString() => UnwrappedContents.ToString();

    ///// <summary>
    ///// Not yet implemented accessor - must be implemented by the inheriting class.
    ///// </summary>
    ///// <param name="index"></param>
    ///// <returns>a <see cref="System.NotImplementedException"/></returns>
    //public abstract object this[int index] { get; }
}