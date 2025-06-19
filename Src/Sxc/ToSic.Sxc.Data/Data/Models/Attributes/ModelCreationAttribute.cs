namespace ToSic.Sxc.Data.Models;

/// <summary>
/// BETA / WIP: Attribute to decorate interfaces to specify a concrete type when creating the model.
/// </summary>
/// <example>
/// ```c#
/// [ModelCreation(Use = typeof(PersonModel))]
/// interface IPersonModel : ICanWrapData
/// {
///   public string Name { get; }
/// }
/// ```
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public sealed class ModelCreationAttribute: Attribute
{
    /// <summary>
    /// The type to use when creating a model of this interface.
    /// </summary>
    /// <remarks>
    /// It **must** match (implement or inherit) the type which is being decorated.
    /// Otherwise, it will throw an exception.
    /// </remarks>
    public Type? Use { get; init; }
}
