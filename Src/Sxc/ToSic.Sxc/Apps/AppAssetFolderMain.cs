using System;

namespace ToSic.Sxc.Apps
{
    internal class AppAssetFolderMain: AppAssetFolder
    {
        internal const string LocationSite = "site";
        internal const string LocationShared = "shared";
        internal const string LocationAuto = "auto";

        /// <summary>
        /// Return true/false or null to allow upstream to do auto-detect
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static bool? DetermineShared(string location)
        {
            switch (location?.ToLowerInvariant() ?? LocationAuto)
            {
                case LocationAuto: return null;
                case LocationShared: return true;
                case LocationSite: return false;
                default: throw new ArgumentException($@"should be null, {LocationAuto}, {LocationSite} or {LocationShared}", nameof(location));
            }
        }


        private readonly IApp _app;
        private readonly bool _shared;

        public AppAssetFolderMain(IApp app, bool shared)
        {
            _app = app;
            _shared = shared;
        }

        public override string Name => _app.Folder;

        public override string Path => _shared ? _app.RelativePathShared : _app.RelativePath;

        public override string PhysicalPath => (_shared ? _app.PhysicalPathShared : _app.PhysicalPath);

        public override string Url => _shared ? _app.PathShared : _app.Path;


    }
}
