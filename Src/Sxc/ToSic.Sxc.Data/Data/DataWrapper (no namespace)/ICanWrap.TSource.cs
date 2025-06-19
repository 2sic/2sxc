using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data;

/// <summary>
/// Marks objects such as custom items or data models, which can receive a _specific_ data-type (entity _or_ typed item) and wrap it.
/// </summary>
/// <remarks>
/// This is more specific than the <see cref="ICanWrapData"/>, since that is just a marker interface.
/// This one specifies that the object has the necessary `Setup()` method to receive the data of the expected type.
/// 
/// Typical use is for custom data such as classes inheriting from <see cref="Custom.Data.CustomItem"/>
/// which takes an entity and then provides a strongly typed wrapper around it.
/// 
/// History
/// 
/// * Introduced in v17.02 under a slightly different name
/// * Made visible in the docs for better understanding in v19.01
/// * The `Setup()` method is still internal, as the signature may still change
/// </remarks>
/// <typeparam name="TSource">
/// The data type which can be accepted.
/// Must be <see cref="IEntity"/> or <see cref="ITypedItem"/> (other types not supported for now).
/// </typeparam>
[InternalApi_DoNotUse_MayChangeWithoutNotice("may change or rename at any time")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICanWrap<in TSource>: ICanWrapData
{
    // TODO: MAKE PRIVATE AGAIN AFTER MOVING TO ToSic.Sxc.Custom

    /// <summary>
    /// Add the data to use for the wrapper.
    /// We are not doing this in the constructor,
    /// because the object needs to have an empty or DI-compatible constructor. 
    /// </summary>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public void Setup(TSource source, IModelFactory modelFactory);
}