using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsUser(CmsContext parent, IMetadataOfSource appReader)
    : CmsContextPartBase<IUser>(parent, parent.CtxSite.User), ICmsUser
{
    public string Email => IsAnonymous ? "" : GetContents().Email;


    public int Id => GetContents().Id;

    public bool IsSiteAdmin => GetContents().IsSiteAdmin;

    public bool IsContentAdmin => GetContents().IsContentAdmin;

    public bool IsSystemAdmin => GetContents().IsSystemAdmin;

    public bool IsSiteDeveloper => GetContents().IsSiteDeveloper;

    public bool IsAnonymous => GetContents().IsAnonymous;

    protected override IMetadataOf GetMetadataOf() 
        => ExtendWithRecommendations(appReader.GetMetadataOf(TargetTypes.User, Id, title: "User (" + Id + ")"));

    public string Name => IsAnonymous ? "" : GetContents().Name;
    public string Username => IsAnonymous ? "" : GetContents().Username;
}