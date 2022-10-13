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

        public int Id => _contents.Id;

        public bool IsSiteAdmin => _contents.IsSiteAdmin;

        public bool IsContentAdmin => _contents.IsContentAdmin;

        public bool IsSystemAdmin => _contents.IsSystemAdmin;

        public bool IsSiteDeveloper => _contents.IsDesigner;

        public bool IsAnonymous => _contents.IsAnonymous;

        protected override IMetadataOf GetMetadataOf() 
            => ExtendWithRecommendations(_appState.GetMetadataOf(TargetTypes.User, Id, "User (" + Id + ")"));

        public string Username => _contents.Username.HasValue() ? _contents.Username : "";
    }
}
