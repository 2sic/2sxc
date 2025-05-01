using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Cms.Users;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsUser(CmsContext parent, IUserModel userModel, IMetadataOfSource appReader)
    : CmsContextPartBase<IUser>(parent, parent.CtxSite.User), ICmsUser
{
    //public string Email => IsAnonymous ? "" : GetContents().Email;
    public string Email => userModel.Email ?? ""; // TODO: not sure if defaulting to empty string is good, as it's probably different from the IUserModel


    public int Id => userModel.Id;
    public Guid Guid => userModel.Guid;
    public DateTime Created => userModel.Created;
    public DateTime Modified => userModel.Modified;

    public bool IsSiteAdmin => userModel.IsSiteAdmin;

    public bool IsContentAdmin => userModel.IsContentAdmin;
    public bool IsContentEditor => userModel.IsContentEditor; 
    public string NameId => userModel.NameId;

    public bool IsSystemAdmin => userModel.IsSystemAdmin;

    public bool IsSiteDeveloper => userModel.IsSiteDeveloper;

    public bool IsAnonymous => userModel.IsAnonymous;

    protected override IMetadataOf GetMetadataOf() 
        => ExtendWithRecommendations(appReader.GetMetadataOf(TargetTypes.User, Id, title: "User (" + Id + ")"));

    public string Name => IsAnonymous ? "" : userModel.Name;
    public string Username => IsAnonymous ? "" : userModel.Username;
    public IEnumerable<IUserRoleModel> Roles => userModel.Roles;
}