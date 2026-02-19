namespace ToSic.Sxc.Cms.Users.Sys;

internal record UserRoleModelOfEntity: ModelOfEntity, IUserRoleModel
{
    public int Id => Entity.EntityId;

    public string Name =>
        Entity.Get<string>(nameof(Name), fallback: "unknown");

    public DateTime Created => Entity.Created;

    public DateTime Modified => Entity.Modified;
}