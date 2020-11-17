using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Engines;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend: HasLog
    {
        #region Constructor / DI

        

        private readonly TemplateHelpers _tmplHelpers;
        private readonly Lazy<AssetEditor> _assetEditorLazy;
        private readonly IServiceProvider _serviceProvider;

        public AppAssetsBackend(TemplateHelpers tmplHelpers, Lazy<AssetEditor> assetEditorLazy, IServiceProvider serviceProvider) : base("Bck.Assets")
        {
            _tmplHelpers = tmplHelpers;
            _assetEditorLazy = assetEditorLazy;
            _serviceProvider = serviceProvider;
        }

        public AppAssetsBackend Init(IApp app, IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _app = app;
            _user = user;
            _tmplHelpers.Init(app, Log);
            return this;
        }

        #endregion

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

        public bool Create(int appId,  string path,  FileContentsDto content, bool global = false)
        {
            Log.Add($"create a#{appId}, path:{path}, global:{global}, cont-length:{content.Content?.Length}");
            path = path.Replace("/", "\\");

            var thisApp = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

            if (content.Content == null)
                content.Content = "";

            path = SanitizePathAndContent(path, content);

            var isAdmin = _user.IsAdmin;
            var assetEditor = _assetEditorLazy.Value.Init(thisApp, path, _user.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow(path);
            return assetEditor.Create(content.Content);
        }

        private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(int appId, int templateId, bool global, string path)
        {
            var wrapLog = Log.Call<AssetEditor>($"{appId}, {templateId}, {global}, {path}");
            var isAdmin = _user.IsAdmin;
            var app = _app;
            if (appId != 0 && appId != app.AppId)
                app = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);
            var assetEditor = templateId != 0 && path == null
                ? _serviceProvider.Build<AssetEditor>().Init(app, templateId, _user.IsSuperUser, isAdmin, Log)
                : _serviceProvider.Build<AssetEditor>().Init(app, path, _user.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor);
        }



    }
}
