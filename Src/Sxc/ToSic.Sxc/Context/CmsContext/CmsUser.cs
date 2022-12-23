using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Context
{
    public class CmsUser: CmsContextPartBase<IUser>, ICmsUser
    {
        public CmsUser(CmsContext parent, AppState appState) : base(parent, parent.CtxSite.User)
        {
            _appState = appState;
        }

        private readonly AppState _appState;

        public string Email => IsAnonymous ? "" : UnwrappedContents.Email;


        public int Id => UnwrappedContents.Id;

        public bool IsSiteAdmin => UnwrappedContents.IsSiteAdmin;

        public bool IsContentAdmin => UnwrappedContents.IsContentAdmin;

        public bool IsSystemAdmin => UnwrappedContents.IsSystemAdmin;

        public bool IsSiteDeveloper => UnwrappedContents.IsDesigner;

        public bool IsAnonymous => UnwrappedContents.IsAnonymous;

        protected override IMetadataOf GetMetadataOf() 
            => ExtendWithRecommendations(_appState.GetMetadataOf(TargetTypes.User, Id, "User (" + Id + ")"));

        public string Name => IsAnonymous ? "" : UnwrappedContents.Name;
        public string Username => IsAnonymous ? "" : UnwrappedContents.Username;
    }
}
