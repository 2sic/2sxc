namespace ToSic.Sxc.Cms.Users.Sys;


internal record UserModelOfEntity : ModelOfEntity, IUserModel
{

    public string? Email => GetThis<string>(null);

    public int Id => Entity.EntityId;

    public Guid Guid => Entity.EntityGuid;

    public DateTime Created => Entity.Created;

    public DateTime Modified => Entity.Modified;

    public bool IsAnonymous => GetThis(false);

    public bool IsSiteAdmin => GetThis(false);

    public bool IsContentAdmin => GetThis(false);

    public bool IsContentEditor => GetThis(false);

    public string? NameId => GetThis<string>(null);

    public bool IsSystemAdmin => GetThis(false);

    public bool IsSiteDeveloper => GetThis(false);

    //IMetadata ICmsUser.Metadata => null;

    public string? Name => GetThis<string>(null);

    public string? Username => GetThis<string>(null);

    //IMetadataOf IHasMetadata.Metadata => null;

    public IEnumerable<IUserRoleModel> Roles =>
        Entity.Children(field: nameof(Roles)).ToModels<UserRoleModelOfEntity>();
        //AsList<UserRoleModelOfEntity>(Entity.Children(field: nameof(Roles)))!;

}