using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Assets;
using ToSic.Sxc.Apps.Assets;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppFilesControllerReal: HasLog<AppFilesControllerReal>, IAppFilesController
    {
        public const string LogSuffix = "AppAss";
        #region Constructor / DI

        public AppFilesControllerReal(
            ISite site,
            IUser user, 
            Lazy<AssetEditor> assetEditorLazy,
            IServiceProvider serviceProvider,
            IAppStates appStates,
            AppPaths appPaths
            ) : base("Bck.Assets")
        {
            _site = site;
            _assetEditorLazy = assetEditorLazy;
            _assetTemplates = new AssetTemplates().Init(Log);
            _serviceProvider = serviceProvider;
            _appStates = appStates;
            _appPaths = appPaths;
            _user = user;
        }

        private readonly ISite _site;
        private readonly Lazy<AssetEditor> _assetEditorLazy;
        private readonly AssetTemplates _assetTemplates;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppStates _appStates;
        private readonly AppPaths _appPaths;
        private readonly IUser _user;

        #endregion

        /// <summary>
        /// Get details and source code
        /// </summary>
        public AssetEditInfo Asset(int appId, int templateId = 0, string path = null, bool global = false)
        {
            var wrapLog = Log.Call<AssetEditInfo>($"asset templ:{templateId}, path:{path}, global:{global}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor.EditInfoWithSource);
        }

        /// <summary>
        /// Save operation - but must be called Asset to match public REST API
        /// </summary>
        public bool Asset(int appId, AssetEditInfo template, int templateId, string path, bool global)
        {
            var wrapLog = Log.Call<bool>($"templ:{templateId}, global:{global}, path:{path}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.Source = template.Code;
            return wrapLog(null, true);
        }

        /// <summary>
        /// Create a new file (if it doesn't exist yet) and optionally prefill it with content
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="path"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="templateKey"></param>
        /// <returns></returns>
        public bool Create(// note: as of 2020-09 the content is never submitted
            int appId,
            string path,
            bool global,
            string templateKey
            // as of 2021-12, all create calls include templateKey
        )
        {
            var assetFromTemplateDto = new AssetFromTemplateDto
            {
                AppId = appId,
                Path = path,
                Global = global,
                TemplateKey = templateKey,
            };
            var wrapLog = Log.Call<bool>($"create a#{assetFromTemplateDto.AppId}, path:{assetFromTemplateDto.Path}, global:{assetFromTemplateDto.Global}, key:{assetFromTemplateDto.TemplateKey}");

            assetFromTemplateDto.Path = assetFromTemplateDto.Path.Replace("/", "\\");

            // ensure all .cshtml start with "_"
            EnsureCshtmlStartWithUnderscore(assetFromTemplateDto);

            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(assetFromTemplateDto);

            // get and prepare template content
            var body = GetTemplateContent(assetFromTemplateDto);

            return wrapLog("Created", assetEditor.Create(body));
        }

        /// <summary>
        /// Get all asset template types
        /// </summary>
        /// <param name="purpose">filter by Purpose when provided</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public TemplatesDto GetTemplates(string purpose, string type)
        {
            var templateInfos = _assetTemplates.GetTemplates();

            // TBD: future purpose implementation
            purpose = (purpose ?? AssetTemplates.ForTemplate).ToLowerInvariant().Trim();
            var defId = AssetTemplates.RazorHybrid.Key;
            if (purpose.Equals(AssetTemplates.ForApi, InvariantCultureIgnoreCase))
                defId = AssetTemplates.ApiHybrid.Key;
            if (purpose.Equals(AssetTemplates.ForSearch, InvariantCultureIgnoreCase))
                defId = AssetTemplates.DnnSearch.Key;

            // For templates we also check the type
            if (purpose.Equals(AssetTemplates.ForTemplate, InvariantCultureIgnoreCase))
            {
                type = type?.ToLowerInvariant().Trim() ?? "";
                if (type.Equals(AssetTemplates.TypeToken, InvariantCultureIgnoreCase))
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
            var app = _appStates.Get(appId);
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
            var app = _appStates.Get(assetFromTemplateDto.AppId);
            var assetEditor = _assetEditorLazy.Value.Init(app, assetFromTemplateDto.Path, assetFromTemplateDto.Global, 0, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow(assetEditor.InternalPath);
            return wrapLog(null, assetEditor);
        }

        public TemplatePreviewDto Preview(int appId, string path, string templateKey, bool b)
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
