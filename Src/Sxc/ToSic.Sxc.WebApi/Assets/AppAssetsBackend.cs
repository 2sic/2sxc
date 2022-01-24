using System;
using System.IO;
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

        public AppAssetsBackend(AppPathHelpers appPathHelpers,
            IUser user, 
            Lazy<AssetEditor> assetEditorLazy,
            IServiceProvider serviceProvider,
            IAppStates appStates) : base("Bck.Assets")
        {

            _appPathHelpers = appPathHelpers;
            _assetEditorLazy = assetEditorLazy;
            _assetTemplates = new AssetTemplates().Init(Log);
            _serviceProvider = serviceProvider;
            _appStates = appStates;
            _user = user;
        }
        private readonly AppPathHelpers _appPathHelpers;
        private readonly Lazy<AssetEditor> _assetEditorLazy;
        private readonly AssetTemplates _assetTemplates;
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

        //[Obsolete("This Method is Deprecated", false)]
        //public bool Create(int appId, string path, FileContentsDto content, string purpose, bool global = false)
        //{
        //    Log.Add($"create a#{appId}, path:{path}, global:{global}, purpose:{purpose}, cont-length:{content.Content?.Length}");
        //    path = path.Replace("/", "\\");

        //    var thisApp = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

        //    if (content.Content == null)
        //        content.Content = "";

        //    path = SanitizePathAndContent(path, content, purpose);

        //    var assetEditor = _assetEditorLazy.Value.Init(thisApp, path, global, 0, Log);
        //    assetEditor.EnsureUserMayEditAssetOrThrow(path);
        //    return assetEditor.Create(content.Content);
        //}

        public bool Create(AssetFromTemplateDto assetFromTemplateDto)
        {
            var wrapLog = Log.Call<bool>($"create a#{assetFromTemplateDto.AppId}, path:{assetFromTemplateDto.Path}, global:{assetFromTemplateDto.Global}, key:{assetFromTemplateDto.TemplateKey}");

            assetFromTemplateDto.Path = assetFromTemplateDto.Path.Replace("/", "\\");

            // ensure all .cshtml start with "_"
            EnsureCshtmlStartWithUnderscore(assetFromTemplateDto);

            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(assetFromTemplateDto);

            // get and prepare template content
            var content = GetTemplateContent(assetFromTemplateDto);

            return wrapLog("Created", assetEditor.Create(content));
        }

        public TemplatesDto GetTemplates(string purpose, string type)
        {
            var templateInfos = _assetTemplates.GetTemplates();

            // TBD: future purpose implementation
            purpose = (purpose ?? AssetTemplates.ForTemplate).ToLowerInvariant().Trim() ?? "";
            var defId = AssetTemplates.RazorHybrid.Key;
            if (purpose.Equals(AssetTemplates.ForApi, StringComparison.InvariantCultureIgnoreCase))
                defId = AssetTemplates.ApiHybrid.Key;
            if (purpose.Equals(AssetTemplates.ForSearch, StringComparison.InvariantCultureIgnoreCase))
                defId = AssetTemplates.DnnSearch.Key;

            // For templates we also check the type
            if (purpose.Equals(AssetTemplates.ForTemplate, StringComparison.InvariantCultureIgnoreCase))
            {
                type = type?.ToLowerInvariant().Trim() ?? "";
                if (type.Equals(AssetTemplates.TypeToken, StringComparison.InvariantCultureIgnoreCase))
                    defId = AssetTemplates.Token.Key;
            }

            return new TemplatesDto
            {
                Default = defId,
                Templates = templateInfos
            };
        }

        private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(int appId, int templateId, bool global, string path)
        {
            var wrapLog = Log.Call<AssetEditor>($"{appId}, {templateId}, {global}, {path}");
            var app = _serviceProvider.Build<Apps.App>().InitNoData(_appStates.IdentityOfApp(appId), Log);
            var assetEditor = _serviceProvider.Build<AssetEditor>();

            // TODO: simplify once we release v13 #cleanUp EOY 2021
            if (path == null)
                assetEditor.Init(app, templateId, Log);
            else
                assetEditor.Init(app, path, global, templateId, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor);
        }

        private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(AssetFromTemplateDto assetFromTemplateDto)
        {
            var wrapLog = Log.Call<AssetEditor>($"a#{assetFromTemplateDto.AppId}, path:{assetFromTemplateDto.Path}, global:{assetFromTemplateDto.Global}, key:{assetFromTemplateDto.TemplateKey}");
            var thisApp = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(AppConstants.AutoLookupZone, assetFromTemplateDto.AppId), Log);
            var assetEditor = _assetEditorLazy.Value.Init(thisApp, assetFromTemplateDto.Path, assetFromTemplateDto.Global, 0, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow(assetEditor.InternalPath);
            return wrapLog(null, assetEditor);
        }

        public TemplatePreviewDto GetPreview(int appId, string path, string templateKey, bool b)
        {
            var wrapLog = Log.Call<TemplatePreviewDto>($"create a#{appId}, path:{path}, global:{b}, key:{templateKey}");
            var templatePreviewDto = new TemplatePreviewDto();

            try
            {
                var assetFromTemplateDto = new AssetFromTemplateDto()
                {
                    AppId = appId,
                    Path = path?.Replace("/", "\\") ?? string.Empty,
                    Global = b,
                    TemplateKey = templateKey,
                };

                // ensure all .cshtml start with "_"
                EnsureCshtmlStartWithUnderscore(assetFromTemplateDto);

                // check if file can be created
                var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(assetFromTemplateDto);

                // check if file already exists
                if (assetEditor.SanitizeFileNameAndCheckIfAssetAlreadyExists())
                    templatePreviewDto.Error = "Asset already exists.";

                // get and prepare template content
                templatePreviewDto.Preview = GetTemplateContent(assetFromTemplateDto);
            }
            catch (Exception e)
            {
                templatePreviewDto.Error = e.Message;
            }
            finally
            {
                // TODO: validate with 2DM that next have sense
                templatePreviewDto.IsValid = string.IsNullOrEmpty(templatePreviewDto.Error);
            }

            return wrapLog("GetPreview", templatePreviewDto);
        }
    }
}
