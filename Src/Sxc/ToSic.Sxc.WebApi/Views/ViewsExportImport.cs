using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.ImportExport.Serialization;
using ToSic.Eav.ImportExport.Validation;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Helpers;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Assets;
using ToSic.Sxc.WebApi.ImportExport;
using ToSic.Sxc.WebApi.Validation;
using Import = ToSic.Eav.Apps.ImportExport.Import;

namespace ToSic.Sxc.WebApi.Views
{
    internal class ViewsExportImport: HasLog
    {
        private readonly IHttp _http;
        private readonly TemplateHelpers _tmplHelpers;
        private readonly IEnvironmentLogger _envLogger;
        private ITenant _tenant;
        private IUser _user;

        public ViewsExportImport(IHttp http, TemplateHelpers tmplHelpers, IEnvironmentLogger envLogger) : base("Bck.Views")
        {
            _http = http;
            _tmplHelpers = tmplHelpers;
            _envLogger = envLogger;
        }

        public ViewsExportImport Init(ITenant tenant, IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _tenant = tenant;
            _user = user;
            return this;
        }

        public HttpResponseMessage DownloadViewAsJson(int appId, int viewId)
        {
            var logCall = Log.Call($"{appId}, {viewId}");
            SecurityHelpers.ThrowIfNotAdmin(_user);
            var app = ImpExpHelpers.GetAppAndCheckZoneSwitchPermissions(_tenant.ZoneId, appId, _user, _tenant.ZoneId, Log);
            var cms = new CmsManager(app, Log);
            var bundle = new BundleEntityWithAssets
            {
                Entity = app.Data[Eav.ImportExport.Settings.TemplateContentType].One(viewId)
            };

            // Attach files
            var view = new View(bundle.Entity, Log);

            _tmplHelpers.Init(app);
            if (!string.IsNullOrEmpty(view.Path))
            {
                TryAddAsset(bundle, _tmplHelpers.ViewPath(view), view.Path);
                var thumb = _tmplHelpers.ViewThumbnail(view);
                if(thumb != null)
                    TryAddAsset(bundle, thumb, _tmplHelpers.ViewIconFileName(view.Path));
            }

            var serializer = new JsonBundleSerializer();
            serializer.Init(cms.AppState, Log);
            var serialized = serializer.Serialize(bundle, 0);

            logCall("ok");
            return Download.BuildDownload(serialized,
                ("View" + "." + bundle.Entity.GetBestTitle() + ImpExpConstants.Extension(ImpExpConstants.Files.json))
                .RemoveNonFilenameCharacters());
        }

        private void TryAddAsset(BundleEntityWithAssets bundle, string webPath, string relativePath)
        {
            if (string.IsNullOrEmpty(webPath)) return;
            var realPath = _http.MapPath(webPath);
            var jsonAssetMan = new JsonAssets();
            var asset1 = jsonAssetMan.Get(realPath, relativePath);
            bundle.Assets.Add(asset1);
        }



        public ImportResultDto ImportView(int zoneId, int appId, List<FileUploadDto> files, string defaultLanguage)
        {
            var callLog = Log.Call<ImportResultDto>($"{zoneId}, {appId}, {defaultLanguage}");

            try
            {
                // 0.1 Check permissions, get the app, 
                var app = ImpExpHelpers.GetAppAndCheckZoneSwitchPermissions(_tenant.ZoneId, appId, _user, _tenant.ZoneId, Log);
                _tmplHelpers.Init(app);

                // 0.2 Verify it's json etc.
                if (files.Any(file => !Json.IsValidJson(file.Contents)))
                    throw new ArgumentException("a file is not json");

                // 1. create the views
                var serializer = new JsonBundleSerializer();
                serializer.Init(State.Get(app), Log);

                var bundles = files.Select(f => serializer.Deserialize(f.Contents)).ToList();

                if (bundles.Any(t => t == null))
                    throw new NullReferenceException("At least one file returned a null-item, something is wrong");

                // 1.1 Verify these are view-entities
                if (!bundles.All(v => v.Entity.Type.Is(Eav.ImportExport.Settings.TemplateContentType)))
                    throw new Exception("At least one of the uploaded items is not a view configuration. " +
                                        "Expected all to be " + Eav.ImportExport.Settings.TemplateContentType);

                // 2. Import the views
                var import = new Import(zoneId, appId, true, parentLog: Log);
                var views = bundles.Select(v => v.Entity as Entity).ToList();
                import.ImportIntoDb(null, views);

                SystemManager.Purge(zoneId, appId, log: Log);

                // 3. Import the attachments
                var assets = bundles.SelectMany(b => b.Assets);
                var assetMan = new JsonAssets();
                foreach (var asset in assets) assetMan.Create(GetRealPath(asset), asset, false);

                // 3. possibly show messages / issues
                return callLog("ok", new ImportResultDto(true));
            }
            catch (Exception ex)
            {
                _envLogger.LogException(ex);
                return callLog("error", new ImportResultDto(false, ex.Message, Message.MessageTypes.Error));
            }
        }

        private string GetRealPath(JsonAsset asset)
        {
            if (!string.IsNullOrEmpty(asset.Storage) && asset.Storage != JsonAsset.StorageApp) return null;
            var root = _tmplHelpers.AppPathRoot(false);
            return Path.Combine(root, asset.Folder, asset.Name);
        }
    }
}
