using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Backend.SaveHelpers;

namespace ToSic.Sxc.WebApi.Tests.SaveHelpers;

/// <summary>
/// Test accessor methods for <see cref="IsUniqueValidator"/>.
/// Provides logic-free pass-through methods to keep production API usage analysis clean.
/// </summary>
internal static class IsUniqueValidatorTestAccessors
{
    /// <summary>
    /// Test accessor for <see cref="IsUniqueValidator.UniqueValueOnly"/>.
    /// </summary>
    public static HttpExceptionAbstraction? UniqueValueOnlyTac(this IsUniqueValidator validator, IEnumerable<IEntity> existingEntities, IEntity pendingEntity, int index = 0)
        => validator.UniqueValueOnly(existingEntities, pendingEntity, index);

    /// <summary>
    /// Test accessor for <see cref="IsUniqueValidator.UniqueValuesOnly"/>.
    /// </summary>
    public static HttpExceptionAbstraction? UniqueValuesOnlyTac(this IsUniqueValidator validator, IEnumerable<IEntity> existingEntities, IReadOnlyCollection<IEntity> pendingEntities)
        => validator.UniqueValuesOnly(existingEntities, pendingEntities);
}