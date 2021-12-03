using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context
{
    public class CmsUser: Wrapper<IUser>, ICmsUser
    {
        private readonly AppState _appState;

        public CmsUser(IUser user, AppState appState) : base(user)
        {
            _appState = appState;
        }

        public int Id => _contents.Id;

        public bool IsSiteAdmin => _contents.IsAdmin;

        public bool IsSystemAdmin => _contents.IsSuperUser;

        public bool IsSiteDeveloper => _contents.IsDesigner;

        public IMetadataOf Metadata
            => _metadata ?? (_metadata = new MetadataOf<string>((int)TargetTypes.CmsItem, CmsMetadata.UserPrefix + Id, _appState));
        private IMetadataOf _metadata;

    }
}
