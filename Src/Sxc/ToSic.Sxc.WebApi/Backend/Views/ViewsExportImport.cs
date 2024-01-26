using System.IO;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Helpers;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.ImportExport.Internal.Xml;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.ImportExport.Validation;
using ToSic.Eav.Integration.Environment;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Security;
using ToSic.Eav.Serialization.Internal;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Eav.WebApi.Validation;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Backend.ImportExport;
using ToSic.Sxc.Blocks.Internal;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Views;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ViewsExportImport : ServiceBase
{
    private readonly IAppPathsMicroSvc _appPathSvc;
    private readonly GenWorkDb<WorkEntitySave> _workEntSave;
    private readonly LazySvc<QueryDefinitionBuilder> _qDefBuilder;
    private readonly IServerPaths _serverPaths;
    private readonly IEnvironmentLogger _envLogger;
    private readonly LazySvc<JsonSerializer> _jsonSerializerLazy;
    //private readonly IAppStates _appStates;
    private readonly AppIconHelpers _appIconHelpers;
    private readonly Generator<ImpExpHelpers> _impExpHelpers;
    private readonly IResponseMaker _responseMaker;
    private readonly ISite _site;
    private readonly IUser _user;

    public ViewsExportImport(
        GenWorkDb<WorkEntitySave> workEntSave,
        IServerPaths serverPaths,
        IEnvironmentLogger envLogger,
        LazySvc<JsonSerializer> jsonSerializerLazy, 
        IContextOfSite context,
        //IAppStates appStates,
        AppIconHelpers appIconHelpers,
        Generator<ImpExpHelpers> impExpHelpers,
        IResponseMaker responseMaker,
        LazySvc<QueryDefinitionBuilder> qDefBuilder,
        IAppPathsMicroSvc appPathSvc) : base("Bck.Views")
    {
        ConnectServices(
            _workEntSave = workEntSave,
            _serverPaths = serverPaths,
            _envLogger = envLogger,
            _jsonSerializerLazy = jsonSerializerLazy,
            //_appStates = appStates,
            _appIconHelpers = appIconHelpers,
            _impExpHelpers = impExpHelpers,
            _responseMaker = responseMaker,
            _qDefBuilder = qDefBuilder,
            _appPathSvc = appPathSvc
        );
        _site = context.Site;
        _user = context.User;
    }

    public THttpResponseType DownloadViewAsJson(int appId, int viewId)
    {
        var logCall = Log.Fn<THttpResponseType>($"{appId}, {viewId}");
        SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log);
        var app = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(_site.ZoneId, appId, _user, _site.ZoneId);
        var bundle = new BundleEntityWithAssets
        {
            Entity = app.StateCache.List.One(viewId).IfOfType(Settings.TemplateContentType) // .Data[Eav.ImportExport.Settings.TemplateContentType].One(viewId)
        };

        var appPaths = _appPathSvc.Init(_site, app);

        // Attach files
        var view = new View(bundle.Entity, new[] { _site.CurrentCultureCode }, Log, _qDefBuilder);

        if (!string.IsNullOrEmpty(view.Path))
        {
            TryAddAsset(bundle, appPaths.ViewPath(view, PathTypes.PhysRelative), view.Path);
            var webPath = _appIconHelpers.IconPathOrNull(appPaths, view, PathTypes.PhysRelative)?.ForwardSlash();
            if(webPath != null)
            {
                var relativePath = webPath.Replace(appPaths.RelativePath.ForwardSlash(), "").TrimPrefixSlash();
                TryAddAsset(bundle, webPath, relativePath);
            }
        }

        var serializer = _jsonSerializerLazy.Value.SetApp(app);
        var serialized = serializer.Serialize(bundle, 0);

        return logCall.ReturnAsOk(_responseMaker.File(serialized,
            ("View" + "." + bundle.Entity.GetBestTitle() + ImpExpConstants.Extension(ImpExpConstants.Files.json))
            .RemoveNonFilenameCharacters()));
    }

    private void TryAddAsset(BundleEntityWithAssets bundle, string webPath, string relativePath)
    {
        if (string.IsNullOrEmpty(webPath)) return;
        var realPath = _serverPaths.FullAppPath(webPath);
        var jsonAssetMan = new JsonAssets();
        var asset1 = jsonAssetMan.Get(realPath, relativePath);
        bundle.Assets.Add(asset1);
    }

        

    public ImportResultDto ImportView(int zoneId, int appId, List<FileUploadDto> files, string defaultLanguage)
    {
        var callLog = Log.Fn<ImportResultDto>($"{zoneId}, {appId}, {defaultLanguage}");

        try
        {
            // 0.1 Check permissions, get the app, 
            var appRead = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(_site.ZoneId, appId, _user, _site.ZoneId);
            var appPaths = _appPathSvc.Init(_site, appRead);

            // 0.2 Verify it's json etc.
            if (files.Any(file => !Json.IsValidJson(file.Contents)))
                throw new ArgumentException("a file is not json");

            // 1. create the views
            var serializer = _jsonSerializerLazy.Value.SetApp(appRead);

            var bundles = files.Select(f => serializer.DeserializeEntityWithAssets(f.Contents)).ToList();

            if (bundles.Any(t => t == null))
                throw new NullReferenceException("At least one file returned a null-item, something is wrong");

            // 1.1 Verify these are view-entities
            if (!bundles.All(v => v.Entity.Type.Is(Settings.TemplateContentType)))
                throw new("At least one of the uploaded items is not a view configuration. " +
                          "Expected all to be " + Settings.TemplateContentType);

            // 2. Import the views
            // todo: construction of this should go into init
            // #ExtractEntitySave - verified
            _workEntSave.New(appRead).Import(bundles.Select(v => v.Entity).ToList());

            // 3. Import the attachments
            var assets = bundles.SelectMany(b => b.Assets);
            var assetMan = new JsonAssets();
            foreach (var asset in assets) assetMan.Create(GetRealPath(appPaths, asset), asset);

            // 3. possibly show messages / issues
            return callLog.ReturnAsOk(new(true));
        }
        catch (Exception ex)
        {
            _envLogger.LogException(ex);
            return callLog.Return(new(false, ex.Message, Message.MessageTypes.Error), "error");
        }
    }

    private string GetRealPath(IAppPaths app, JsonAsset asset)
    {
        if (!string.IsNullOrEmpty(asset.Storage) && asset.Storage != JsonAsset.StorageApp) return null;
        var root = app.PhysicalPathSwitch(false);
        return Path.Combine(root, asset.Folder, asset.Name);
    }
}