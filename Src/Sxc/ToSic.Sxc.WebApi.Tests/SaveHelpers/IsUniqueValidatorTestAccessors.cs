using ToSic.Eav.Data;
using ToSic.Sxc.Backend.SaveHelpers;

namespace ToSic.Sxc.WebApi.Tests.SaveHelpers;

/// <summary>
/// Test accessor methods for <see cref="IsUniqueValidator"/>.
/// Provides logic-free pass-through methods to keep production API usage analysis clean.
/// </summary>
internal static class IsUniqueValidatorTestAccessors
{
    /// <summary>
    /// Test accessor for <see cref="IsUniqueValidator.UniqueValuesOnly"/>.
    /// </summary>
    public static Exception? UniqueValuesOnlyTac(this IsUniqueValidator validator, IReadOnlyCollection<IEntity> pendingEntities)
        => validator.UniqueValuesOnly(pendingEntities);
}
