using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CmsUser: CmsContextPartBase<IUser>, ICmsUser
{
    public CmsUser(CmsContext parent, IMetadataOfSource appState) : base(parent, parent.CtxSite.User)
    {
        _appState = appState;
    }

    private readonly IMetadataOfSource _appState;

    public string Email => IsAnonymous ? "" : GetContents().Email;


    public int Id => GetContents().Id;

    public bool IsSiteAdmin => GetContents().IsSiteAdmin;

    public bool IsContentAdmin => GetContents().IsContentAdmin;

    public bool IsSystemAdmin => GetContents().IsSystemAdmin;

    public bool IsSiteDeveloper => GetContents().IsSiteDeveloper;

    public bool IsAnonymous => GetContents().IsAnonymous;

    protected override IMetadataOf GetMetadataOf() 
        => ExtendWithRecommendations(_appState.GetMetadataOf(TargetTypes.User, Id, "User (" + Id + ")"));

    public string Name => IsAnonymous ? "" : GetContents().Name;
    public string Username => IsAnonymous ? "" : GetContents().Username;
}