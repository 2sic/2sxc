using System.IO;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using static ToSic.Eav.Apps.AppConstants;

namespace ToSic.Sxc.Apps
{
    internal class AppAssetThumbnail : AppAssetFile
    {
        private readonly IApp _app;
        private readonly LazySvc<GlobalPaths> _globalPaths;

        public AppAssetThumbnail(IApp app, LazySvc<GlobalPaths> globalPaths)
        {
            _app = app;
            _globalPaths = globalPaths;
        }

        public override string Url => _url.Get(() =>
        {
            // Primary app - we only PiggyBack cache the icon in this case
            // Because otherwise the icon could get moved, and people would have a hard time seeing the effect
            if (_app.NameId == Eav.Constants.PrimaryAppGuid)
                return _app.AppState.GetPiggyBack("app-thumbnail-primary",
                    () => _globalPaths.Value.GlobalPathTo(AppPrimaryIconFile, PathTypes.Link));

            // standard app (not global) try to find app-icon in its (portal) app folder
            if (!_app.AppState.IsShared() && File.Exists($"{_app.PhysicalPath}/{AppIconFile}")) 
                return $"{_app.Path}/{AppIconFile}";

            // global app (and standard app without app-icon in its portal folder) looks for app-icon in global shared location 
            if (File.Exists($"{_app.PhysicalPathShared}/{AppIconFile}"))
                return $"{_app.PathShared}/{AppIconFile}";

            return null;

        });
        private readonly GetOnce<string> _url = new GetOnce<string>();
    }
}
