using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Assets;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Assets;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Admin.AppFiles;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class AppFilesControllerReal: ServiceBase, IAppFilesController
{
    public const string LogSuffix = "AppAss";
    #region Constructor / DI

    public AppFilesControllerReal(
        ISite site,
        IUser user, 
        Generator<AssetEditor> assetEditorGenerator,
        IAppStates appStates,
        IAppPathsMicroSvc appPaths
    ) : base("Bck.Assets")
    {
            
        _site = site;
        _user = user;
        ConnectServices(
            _assetEditorGenerator = assetEditorGenerator,
            _assetTemplates = new AssetTemplates(),
            _appStates = appStates,
            _appPaths = appPaths
        );
    }

    private readonly ISite _site;
    private readonly Generator<AssetEditor> _assetEditorGenerator;
    private readonly AssetTemplates _assetTemplates;
    private readonly IAppStates _appStates;
    private readonly IAppPathsMicroSvc _appPaths;
    private readonly IUser _user;

    #endregion

    /// <summary>
    /// Get details and source code
    /// </summary>
    public AssetEditInfo Asset(int appId, int templateId = 0, string path = null, bool global = false)
    {
        var wrapLog = Log.Fn<AssetEditInfo>($"asset templ:{templateId}, path:{path}, global:{global}");
        var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
        assetEditor.EnsureUserMayEditAssetOrThrow();
        return wrapLog.Return(assetEditor.EditInfoWithSource);
    }

    /// <summary>
    /// Save operation - but must be called Asset to match public REST API
    /// </summary>
    public bool Asset(int appId, AssetEditInfo template, int templateId, string path, bool global)
    {
        var wrapLog = Log.Fn<bool>($"templ:{templateId}, global:{global}, path:{path}");
        var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
        assetEditor.Source = template.Code;
        return wrapLog.ReturnTrue();
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
        var wrapLog = Log.Fn<bool>($"create a#{assetFromTemplateDto.AppId}, path:{assetFromTemplateDto.Path}, global:{assetFromTemplateDto.Global}, key:{assetFromTemplateDto.TemplateKey}");

        EnsureRequiredFolder(assetFromTemplateDto);

        var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(assetFromTemplateDto);

        // get and prepare template content
        var body = GetTemplateContent(assetFromTemplateDto);

        return wrapLog.Return(assetEditor.Create(body), "Created");
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
        var defId = AssetTemplates.RazorHybrid.Key;
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

        return new TemplatesDto
        {
            Default = defId,
            Templates = templateInfos
        };
    }

    private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(int appId, int templateId, bool global, string path)
    {
        var wrapLog = Log.Fn<AssetEditor>($"{appId}, {templateId}, {global}, {path}");
        var app = _appStates.GetReader(appId);
        var assetEditor = _assetEditorGenerator.New();

        assetEditor.Init(app, path, global, templateId);
        assetEditor.EnsureUserMayEditAssetOrThrow();
        return wrapLog.Return(assetEditor);
    }

    private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(AppFileDto assetFromTemplateDto)
    {
        var wrapLog = Log.Fn<AssetEditor>($"a#{assetFromTemplateDto.AppId}, path:{assetFromTemplateDto.Path}, global:{assetFromTemplateDto.Global}, key:{assetFromTemplateDto.TemplateKey}");
        var app = _appStates.GetReader(assetFromTemplateDto.AppId);
        var assetEditor = _assetEditorGenerator.New().Init(app, assetFromTemplateDto.Path, assetFromTemplateDto.Global, 0);
        assetEditor.EnsureUserMayEditAssetOrThrow(assetEditor.InternalPath);
        return wrapLog.Return(assetEditor);
    }

    public TemplatePreviewDto Preview(int appId, string path, string templateKey, bool b)
    {
        var wrapLog = Log.Fn<TemplatePreviewDto>($"create a#{appId}, path:{path}, global:{b}, key:{templateKey}");
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

        return wrapLog.Return(templatePreviewDto, "GetPreview");
    }
}