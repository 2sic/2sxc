﻿namespace ToSic.Sxc.Data;

/// <summary>
/// This is just a helper interface.
/// Reason is that we have different types of objects representing real entity-data such as:
/// 
/// * <see cref="IDynamicEntity"/>
/// * <see cref="ITypedItem"/>
///
/// To make sure that APIs which use this have a consistent structure, these objects all implement this interface.
/// </summary>
[PrivateApi("Was InternalApi till v17, but Just FYI")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICanBeItem: ICanBeEntity
{
    // #RemoveBlocksIRenderService
    ///// <summary>
    ///// Important: Always implement explicitly, so it doesn't appear in the API
    ///// </summary>
    ///// <returns></returns>
    //[PrivateApi]
    //[ShowApiWhenReleased(ShowApiMode.Never)]
    //object? TryGetBlock();

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    ITypedItem Item { get; }
}