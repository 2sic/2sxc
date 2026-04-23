using ToSic.Eav.Data;

namespace ToSic.Sxc.Backend.SaveHelpers;

[PrivateApi]
public record SaveEntityValidationContext(
    IReadOnlyCollection<IEntity> ExistingEntities,
    IEntity Entity,
    int Index
);