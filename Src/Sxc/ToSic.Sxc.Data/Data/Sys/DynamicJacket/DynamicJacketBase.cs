﻿using System.Collections;
using System.Dynamic;
using ToSic.Eav.Data.Sys;
using ToSic.Sxc.Data.Sys.Json;
using ToSic.Sxc.Data.Sys.Wrappers;

namespace ToSic.Sxc.Data.Sys.DynamicJacket;

/// <summary>
/// Base class for DynamicJackets. You won't use this, just included in the docs. <br/>
/// To check if something is an array or an object, use "IsArray"
/// </summary>
[PrivateApi("was Internal-API till v17 - just use the objects from AsDynamic, don't use this directly")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract class DynamicJacketBase(CodeJsonWrapper wrapper): DynamicObject, IReadOnlyList<object?>, ISxcDynamicObject, ICanGetByName, IHasPropLookup, IHasJsonSource
{
    #region Constructor / Setup

    [PrivateApi] internal CodeJsonWrapper Wrapper { get; } = wrapper;

    [PrivateApi]
    internal abstract IPreWrap PreWrap { get; }

    [PrivateApi]
    IPropertyLookup IHasPropLookup.PropertyLookup => PreWrap;

    [PrivateApi]
    object IHasJsonSource.JsonSource() => PreWrap.JsonSource();

    #endregion

    /// <summary>
    /// Check if it's an array.
    /// </summary>
    /// <returns>True if an array/list, false if an object.</returns>
    public abstract bool IsList { get; }

    /// <summary>
    /// Enable enumeration. When going through objects (properties) it will return the keys, not the values. <br/>
    /// Use the [key] accessor to get the values as <see cref="DynamicJacketList"/> or <see cref="Data"/>
    /// </summary>
    /// <returns></returns>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public abstract IEnumerator<object?> GetEnumerator();


    /// <inheritdoc />
    [PrivateApi]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public dynamic? Get(string name) => PreWrap.TryGetWrap(name).Result;

    /// <summary>
    /// Count array items or object properties
    /// </summary>
    public abstract int Count { get; }

    /// <summary>
    /// Not yet implemented accessor - must be implemented by the inheriting class.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>a <see cref="System.NotImplementedException"/></returns>
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public abstract object? this[int index] { get; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

    /// <summary>
    /// Fake property binder - just ensure that simple properties don't cause errors. <br/>
    /// Must be overriden in inheriting objects
    /// like <see cref="DynamicJacketList"/>, <see cref="DynamicJacket"/>
    /// </summary>
    /// <param name="binder">.net binder object</param>
    /// <param name="result">always null, unless overriden</param>
    /// <returns>always returns true, to avoid errors</returns>
    [PrivateApi]
    public abstract override bool TryGetMember(GetMemberBinder binder, out object? result);
        
}