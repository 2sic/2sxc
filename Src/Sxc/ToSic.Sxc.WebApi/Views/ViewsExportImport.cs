using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DI;
using ToSic.Eav.ImportExport;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.ImportExport.Serialization;
using ToSic.Eav.ImportExport.Validation;
using ToSic.Lib.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Security;
using ToSic.Eav.WebApi.Validation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Views
{
    public class ViewsExportImport<THttpResponseType> : HasLog
    {
        private readonly IServerPaths _serverPaths;
        private readonly IEnvironmentLogger _envLogger;
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly Lazy<JsonBundleSerializer> _jsonBundleLazy;
        private readonly IAppStates _appStates;
        private readonly AppIconHelpers _appIconHelpers;
        private readonly GeneratorLog<ImpExpHelpers> _impExpHelpers;
        private readonly ResponseMaker<THttpResponseType> _responseMaker;
        private readonly ISite _site;
        private readonly IUser _user;

        public ViewsExportImport(IServerPaths serverPaths,
            IEnvironmentLogger envLogger,
            Lazy<CmsManager> cmsManagerLazy, 
            Lazy<JsonBundleSerializer> jsonBundleLazy, 
            IContextOfSite context,
            IAppStates appStates,
            AppIconHelpers appIconHelpers,
            GeneratorLog<ImpExpHelpers> impExpHelpers,
            ResponseMaker<THttpResponseType> responseMaker
            ) : base("Bck.Views")
        {
            _serverPaths = serverPaths;
            _envLogger = envLogger;
            _cmsManagerLazy = cmsManagerLazy;
            _jsonBundleLazy = jsonBundleLazy;
            _appStates = appStates;
            _appIconHelpers = appIconHelpers;
            _impExpHelpers = impExpHelpers.SetLog(Log);
            _responseMaker = responseMaker;

            _site = context.Site;
            _user = context.User;
        }

        public THttpResponseType DownloadViewAsJson(int appId, int viewId)
        {
            var logCall = Log.Fn<THttpResponseType>($"{appId}, {viewId}");
            SecurityHelpers.ThrowIfNotAdmin(_user.IsSiteAdmin);
            var app = _impExpHelpers.New.GetAppAndCheckZoneSwitchPermissions(_site.ZoneId, appId, _user, _site.ZoneId);
            var cms = _cmsManagerLazy.Value.Init(app, Log);
            var bundle = new BundleEntityWithAssets
            {
                Entity = app.Data[Eav.ImportExport.Settings.TemplateContentType].One(viewId)
            };

            // Attach files
            var view = new View(bundle.Entity, _site.CurrentCultureCode, Log);

            _appIconHelpers.Init(Log);
            if (!string.IsNullOrEmpty(view.Path))
            {
                TryAddAsset(bundle, app.ViewPath(view, PathTypes.PhysRelative), view.Path);
                var thumb = _appIconHelpers.IconPathOrNull(app, view, PathTypes.PhysRelative);
                if(thumb != null)
                    TryAddAsset(bundle, thumb, thumb);
            }

            var serializer = _jsonBundleLazy.Value;
            serializer.Init(cms.AppState, Log);
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
                var app = _impExpHelpers.New.GetAppAndCheckZoneSwitchPermissions(_site.ZoneId, appId, _user, _site.ZoneId);
                //_appHelpers.Init(app, Log);

                // 0.2 Verify it's json etc.
                if (files.Any(file => !Json.IsValidJson(file.Contents)))
                    throw new ArgumentException("a file is not json");

                // 1. create the views
                var serializer = _jsonBundleLazy.Value;
                serializer.Init(_appStates.Get(app), Log);

                var bundles = files.Select(f => serializer.Deserialize(f.Contents)).ToList();

                if (bundles.Any(t => t == null))
                    throw new NullReferenceException("At least one file returned a null-item, something is wrong");

                // 1.1 Verify these are view-entities
                if (!bundles.All(v => v.Entity.Type.Is(Eav.ImportExport.Settings.TemplateContentType)))
                    throw new Exception("At least one of the uploaded items is not a view configuration. " +
                                        "Expected all to be " + Eav.ImportExport.Settings.TemplateContentType);

                // 2. Import the views
                // todo: construction of this should go into init
                _cmsManagerLazy.Value.Init(app.AppId, Log).Entities.Import(bundles.Select(v => v.Entity).ToList());

                // 3. Import the attachments
                var assets = bundles.SelectMany(b => b.Assets);
                var assetMan = new JsonAssets();
                foreach (var asset in assets) assetMan.Create(GetRealPath(app, asset), asset);

                // 3. possibly show messages / issues
                return callLog.ReturnAsOk(new ImportResultDto(true));
            }
            catch (Exception ex)
            {
                _envLogger.LogException(ex);
                return callLog.Return(new ImportResultDto(false, ex.Message, Message.MessageTypes.Error), "error");
            }
        }

        private string GetRealPath(Apps.IApp app, JsonAsset asset)
        {
            if (!string.IsNullOrEmpty(asset.Storage) && asset.Storage != JsonAsset.StorageApp) return null;
            var root = app.PhysicalPathSwitch(false);
            return Path.Combine(root, asset.Folder, asset.Name);
        }
    }
}
