using System.IO;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.WebApi.Assets;
using ToSic.Sxc.Apps.Internal.Assets;
using ToSic.Sxc.Code.Internal.HotBuild;
using static System.StringComparison;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class AppFilesControllerReal: ServiceBase, IAppFilesController
{
    public const string LogSuffix = "AppAss";
    #region Constructor / DI

    public AppFilesControllerReal(
        ISite site,
        IUser user, 
        Generator<AssetEditor> assetEditorGenerator,
        IAppReaderFactory appReaders,
        LazySvc<CodeControllerReal> codeController,
        LazySvc<AppCodeLoader> appCodeLoader,
        IAppPathsMicroSvc appPathsFactoryTemp
    ) : base("Bck.Assets")
    {
        _site = site;
        _user = user;
        ConnectLogs([
            _assetEditorGenerator = assetEditorGenerator,
            _assetTemplates = new(),
            _appReaders = appReaders,
            _codeController = codeController,
            _appCodeLoader = appCodeLoader,
            _appPathsFactoryTemp = appPathsFactoryTemp
        ]);
    }

    private readonly ISite _site;
    private readonly Generator<AssetEditor> _assetEditorGenerator;
    private readonly AssetTemplates _assetTemplates;
    private readonly IAppReaderFactory _appReaders;
    private readonly IAppPathsMicroSvc _appPathsFactoryTemp;
    private IAppPaths _appPaths;
    private readonly IUser _user;
    private readonly LazySvc<CodeControllerReal> _codeController;
    private readonly LazySvc<AppCodeLoader> _appCodeLoader;

    #endregion

    /// <summary>
    /// Get details and source code
    /// </summary>
    public AssetEditInfo Asset(int appId, int templateId = 0, string path = null, bool global = false)
    {
        var l = Log.Fn<AssetEditInfo>($"asset templ:{templateId}, path:{path}, global:{global}");
        var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
        assetEditor.EnsureUserMayEditAssetOrThrow();
        return l.Return(assetEditor.EditInfoWithSource);
    }

    /// <summary>
    /// Save operation - but must be called Asset to match public REST API
    /// </summary>
    public bool Asset(int appId, AssetEditInfo template, int templateId, string path, bool global)
    {
        var l = Log.Fn<bool>($"templ:{templateId}, global:{global}, path:{path}");
        var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
        assetEditor.Source = template.Code;
        return l.ReturnTrue();
    }

    /// <summary>
    /// Create a new file (if it doesn't exist yet) and optionally prefill it with content
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="path"></param>
    /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
    /// <param name="templateKey"></param>
    /// <returns></returns>
    public bool Create(int appId, string path, bool global, string templateKey)
    {
        var assetFromTemplateDto = new AppFileDto
        {
            AppId = appId,
            Path = path,
            Global = global,
            TemplateKey = templateKey,
        };
        var l = Log.Fn<bool>($"create a#{assetFromTemplateDto.AppId}, path:{assetFromTemplateDto.Path}, global:{assetFromTemplateDto.Global}, key:{assetFromTemplateDto.TemplateKey}");

        EnsureRequiredFolder(assetFromTemplateDto);

        var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(assetFromTemplateDto);

        // get and prepare template content
        var body = GetTemplateContent(assetFromTemplateDto);

        return l.Return(assetEditor.Create(body), "Created");
    }

    private static void EnsureRequiredFolder(AppFileDto assetFromTemplateDto)
    {
        assetFromTemplateDto.Path = assetFromTemplateDto.Path.Replace("/", "\\");

        // ensure that DataSource is in DataSources folder
        if (assetFromTemplateDto.TemplateKey == AssetTemplates.DataSourceHybrid.Key)
        {
            var directoryName = Path.GetDirectoryName(assetFromTemplateDto.Path) ?? string.Empty;
            var fileName = Path.GetFileName(assetFromTemplateDto.Path) ?? string.Empty;
            if (!directoryName.StartsWith(AssetTemplates.DataSourceHybrid.Folder) && !directoryName.Contains(AssetTemplates.DataSourceHybrid.Folder))
                assetFromTemplateDto.Path = Path.Combine(AssetTemplates.DataSourceHybrid.Folder, fileName);
        }
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
        var defId = AssetTemplates.RazorTyped.Key;
        if (purpose.Equals(AssetTemplates.ForApi, InvariantCultureIgnoreCase))
            defId = AssetTemplates.ApiHybrid.Key;
        if (purpose.Equals(AssetTemplates.ForDataSource, InvariantCultureIgnoreCase))
            defId = AssetTemplates.DataSourceHybrid.Key;
        if (purpose.Equals(AssetTemplates.ForSearch, InvariantCultureIgnoreCase))
            defId = AssetTemplates.DnnSearch.Key;

        // For templates we also check the type
        if (purpose.Equals(AssetTemplates.ForTemplate, InvariantCultureIgnoreCase))
        {
            type = type?.ToLowerInvariant().Trim() ?? "";
            if (type.Equals(AssetTemplates.TypeToken, InvariantCultureIgnoreCase))
                defId = AssetTemplates.Token.Key;
        }

        return new()
        {
            Default = defId,
            Templates = templateInfos
        };
    }

    private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(int appId, int templateId, bool global, string path)
    {
        var l = Log.Fn<AssetEditor>($"{appId}, {templateId}, {global}, {path}");
        var app = _appReaders.Get(appId);
        var assetEditor = _assetEditorGenerator.New();

        assetEditor.Init(app, path, global, templateId);
        assetEditor.EnsureUserMayEditAssetOrThrow();
        return l.Return(assetEditor);
    }

    private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(AppFileDto assetFromTemplateDto)
    {
        var l = Log.Fn<AssetEditor>($"a#{assetFromTemplateDto.AppId}, path:{assetFromTemplateDto.Path}, global:{assetFromTemplateDto.Global}, key:{assetFromTemplateDto.TemplateKey}");
        var app = _appReaders.Get(assetFromTemplateDto.AppId);
        var assetEditor = _assetEditorGenerator.New().Init(app, assetFromTemplateDto.Path, assetFromTemplateDto.Global, 0);
        assetEditor.EnsureUserMayEditAssetOrThrow(assetEditor.InternalPath);
        return l.Return(assetEditor);
    }

    public TemplatePreviewDto Preview(int appId, string path, string templateKey, bool b)
    {
        var l = Log.Fn<TemplatePreviewDto>($"create a#{appId}, path:{path}, global:{b}, key:{templateKey}");
        var templatePreviewDto = new TemplatePreviewDto();

        try
        {
            var assetFromTemplateDto = new AppFileDto
            {
                AppId = appId,
                Path = path?.Replace("/", "\\") ?? string.Empty,
                Global = b,
                TemplateKey = templateKey,
            };

            EnsureRequiredFolder(assetFromTemplateDto);

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

        return l.Return(templatePreviewDto, "GetPreview");
    }
}