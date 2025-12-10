using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Eav.ImportExport.Integration;
using ToSic.Eav.ImportExport.Json.Sys;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.Xml;
using ToSic.Eav.Persistence.Sys.Logging;
using ToSic.Eav.Serialization.Sys;
using ToSic.Eav.WebApi.Sys.Helpers.Validation;
using ToSic.Eav.WebApi.Sys.Security;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Backend.ImportExport;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Views;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ViewsExportImport(
    GenWorkDb<WorkEntitySave> workEntSave,
    IServerPaths serverPaths,
    IEnvironmentLogger envLogger,
    LazySvc<JsonSerializer> jsonSerializerLazy,
    IContextOfSite context,
    AppIconHelpers appIconHelpers,
    Generator<ImpExpHelpers> impExpHelpers,
    IResponseMaker responseMaker,
    Generator<QueryDefinitionBuilder> qDefBuilder,
    IAppPathsMicroSvc appPathSvc)
    : ServiceBase("Bck.Views",
        connect:
        [
            workEntSave, serverPaths, envLogger, jsonSerializerLazy, appIconHelpers, impExpHelpers, responseMaker,
            qDefBuilder, appPathSvc
        ])
{
    public THttpResponseType DownloadViewAsJson(int appId, int viewId)
    {
        var logCall = Log.Fn<THttpResponseType>($"{appId}, {viewId}");
        SecurityHelpers.ThrowIfNotSiteAdmin(context.User, Log);
        var appReader = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(context.Site.ZoneId, appId, context.User, context.Site.ZoneId);
        var bundle = new BundleEntityWithAssets
        {
            Entity = appReader.List.One(viewId)!.IfOfType(Settings.TemplateContentType)!
        };

        var appPaths = appPathSvc.Get(appReader, context.Site);

        // Attach files
        var view = new View(bundle.Entity!, [context.Site.CurrentCultureCode], Log, qDefBuilder);

        if (!string.IsNullOrEmpty(view.Path))
        {
            bundle = TryAddAsset(bundle, appPaths.ViewPath(view, PathTypes.PhysRelative), view.Path);
            var webPath = appIconHelpers.IconPathOrNull(appPaths, view, PathTypes.PhysRelative)?.ForwardSlash();
            if(webPath != null)
            {
                var relativePath = webPath.Replace(appPaths.RelativePath.ForwardSlash(), "").TrimPrefixSlash();
                bundle = TryAddAsset(bundle, webPath, relativePath);
            }
        }

        var serializer = jsonSerializerLazy.Value.SetApp(appReader);
        var serialized = serializer.Serialize(bundle, 0);

        return logCall.ReturnAsOk(responseMaker.File(serialized,
            ("View" + "." + bundle.Entity.GetBestTitle() + ImpExpConstants.Extension(ImpExpConstants.Files.json))
            .RemoveNonFilenameCharacters()));
    }

    private BundleEntityWithAssets TryAddAsset(BundleEntityWithAssets bundle, string webPath, string relativePath)
    {
        if (string.IsNullOrEmpty(webPath))
            return bundle;
        var realPath = serverPaths.FullAppPath(webPath);
        var jsonAssetMan = new JsonAssets();
        var asset1 = jsonAssetMan.Get(realPath, relativePath, JsonAsset.StorageApp);
        bundle = bundle with
        {
            Assets = bundle.Assets.Append(asset1).ToListOpt()
        };
        return bundle;
    }

        

    public ImportResultDto ImportView(int zoneId, int appId, List<FileUploadDto> files, string defaultLanguage)
    {
        var l = Log.Fn<ImportResultDto>($"{zoneId}, {appId}, {defaultLanguage}");

        try
        {
            // 0.1 Check permissions, get the app, 
            var appRead = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(context.Site.ZoneId, appId, context.User, context.Site.ZoneId);
            var appPaths = appPathSvc.Get(appRead, context.Site);

            // 0.2 Verify it's json etc.
            if (files.Any(file => !Json.IsValidJson(file.Contents)))
                throw new ArgumentException("a file is not json");

            // 1. create the views
            var serializer = jsonSerializerLazy.Value.SetApp(appRead);

            var bundles = files.Select(f => serializer.DeserializeEntityWithAssets(f.Contents)).ToList();

            if (bundles.Any(t => t == null!))
                throw new NullReferenceException("At least one file returned a null-item, something is wrong");

            // 1.1 Verify these are view-entities
            if (!bundles.All(v => v.Entity.Type.Is(Settings.TemplateContentType)))
                throw new("At least one of the uploaded items is not a view configuration. " +
                          "Expected all to be " + Settings.TemplateContentType);

            // 2. Import the views
            // todo: construction of this should go into init
            workEntSave.New(appRead).Import(bundles.Select(v => v.Entity).ToList());

            // 3. Import the attachments
            var assets = bundles.SelectMany(b => b.Assets);
            var assetMan = new JsonAssets();
            foreach (var asset in assets)
                assetMan.Create(GetRealPath(appPaths, asset), asset);

            // 3. possibly show messages / issues
            return l.ReturnAsOk(new(true));
        }
        catch (Exception ex)
        {
            envLogger.LogException(ex);
            return l.Return(new(false, ex.Message, Message.MessageTypes.Error), "error");
        }
    }

    private string? GetRealPath(IAppPaths app, JsonAsset asset)
    {
        if (!string.IsNullOrEmpty(asset.Storage) && asset.Storage != JsonAsset.StorageApp)
            return null;
        var root = app.PhysicalPathSwitch(false);
        return Path.Combine(root, asset.Folder!, asset.Name);
    }
}