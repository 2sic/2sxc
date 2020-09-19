using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Web;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Assets
{
    internal partial class AppAssetsBackend: HasLog
    {
        private readonly IHttp _http;

        public AppAssetsBackend(IHttp http) : base("Bck.Assets")
        {
            _http = http;
        }

        public AppAssetsBackend Init(IApp app, IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _app = app;
            _user = user;
            return this;
        }

        private IUser _user;
        private IApp _app;

        public AssetEditInfo Get(int templateId = 0, string path = null, bool global = false, int appId = 0)
        {
            var wrapLog = Log.Call<AssetEditInfo>($"asset templ:{templateId}, path:{path}, global:{global}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor.EditInfoWithSource);
        }


        public bool Save(AssetEditInfo template,  int templateId = 0,  bool global = false,  string path = null,  int appId = 0)
        {
            var wrapLog = Log.Call<bool>($"templ:{templateId}, global:{global}, path:{path}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.Source = template.Code;
            return wrapLog(null, true);
        }

        public bool Create( int appId,  string path,  FileContentsDto content, bool global = false)
        {
            Log.Add($"create a#{appId}, path:{path}, global:{global}, cont-length:{content.Content?.Length}");
            path = path.Replace("/", "\\");

            var thisApp = Factory.Resolve<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

            if (content.Content == null)
                content.Content = "";

            path = SanitizePathAndContent(path, content);

            var isAdmin = _user.IsAdmin;
            var assetEditor = new AssetEditor(thisApp, path, _user.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow(path);
            return assetEditor.Create(content.Content);
        }


    }
}
