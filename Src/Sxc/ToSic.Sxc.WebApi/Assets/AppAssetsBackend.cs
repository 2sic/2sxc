using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend: HasLog<AppAssetsBackend>
    {

        #region Constructor / DI

        public AppAssetsBackend(TemplateHelpers templateHelpers,
            IUser user, 
            Lazy<AssetEditor> assetEditorLazy, 
            IAssetTemplates assetTemplates, 
            IServiceProvider serviceProvider,
            IAppStates appStates) : base("Bck.Assets")
        {

            _templateHelpers = templateHelpers;
            _assetEditorLazy = assetEditorLazy;
            _assetTemplates = assetTemplates;
            _serviceProvider = serviceProvider;
            _appStates = appStates;
            _user = user;
        }
        private readonly TemplateHelpers _templateHelpers;
        private readonly Lazy<AssetEditor> _assetEditorLazy;
        private readonly IAssetTemplates _assetTemplates;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppStates _appStates;
        private readonly IUser _user;

        #endregion


        public AssetEditInfo Get(int appId, int templateId, string path, bool global)
        {
            var wrapLog = Log.Call<AssetEditInfo>($"asset templ:{templateId}, path:{path}, global:{global}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor.EditInfoWithSource);
        }


        public bool Save(int appId, AssetEditInfo template, int templateId, bool global, string path)
        {
            var wrapLog = Log.Call<bool>($"templ:{templateId}, global:{global}, path:{path}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.Source = template.Code;
            return wrapLog(null, true);
        }

        public bool Create(int appId, string path, FileContentsDto content, string purpose, bool global = false)
        {
            Log.Add($"create a#{appId}, path:{path}, global:{global}, purpose:{purpose}, cont-length:{content.Content?.Length}");
            path = path.Replace("/", "\\");

            var thisApp = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

            if (content.Content == null)
                content.Content = "";

            path = SanitizePathAndContent(path, content, purpose);

            var isAdmin = _user.IsAdmin;
            var assetEditor = _assetEditorLazy.Value.Init(thisApp, path, _user.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow(path);
            return assetEditor.Create(content.Content);
        }

        public bool Create(AssetFromTemplateDto assetFromTemplateDto)
        {
            var content = new FileContentsDto(); // empty content
            var purpose = Templates.GetTemplates().First(t => t.Key == assetFromTemplateDto.TemplateKey).Purpose;
            return Create(assetFromTemplateDto.AppId, assetFromTemplateDto.Path, content, purpose, assetFromTemplateDto.Global);
        }

        public TemplatesDto GetTemplates(string templateKey)
        {
            var templateInfos = Templates.GetTemplates().Where(t => t.Key.Equals(templateKey, StringComparison.InvariantCultureIgnoreCase)).ToList();
            
            // prefill template body
            foreach (var templateInfo in templateInfos)
                templateInfo.Body = _assetTemplates.GetTemplate(templateInfo.Type);

            return new TemplatesDto {Templates = templateInfos};
        }

        private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(int appId, int templateId, bool global, string path)
        {
            var wrapLog = Log.Call<AssetEditor>($"{appId}, {templateId}, {global}, {path}");
            var isAdmin = _user.IsAdmin;
            var app = _serviceProvider.Build<Apps.App>().InitNoData(_appStates.Identity(null, appId), Log);
            var assetEditor = templateId != 0 && path == null
                ? _serviceProvider.Build<AssetEditor>().Init(app, templateId, _user.IsSuperUser, isAdmin, Log)
                : _serviceProvider.Build<AssetEditor>().Init(app, path, _user.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor);
        }

    }
}
