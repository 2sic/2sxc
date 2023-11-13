using System;
using ToSic.Eav.Apps.Paths;

namespace ToSic.Sxc.Apps
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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


        internal readonly AppPaths AppPaths;
        private readonly bool _shared;

        public AppAssetFolderMain(AppPaths appPaths, string folder, bool shared)
        {
            Name = folder;
            AppPaths = appPaths;
            _shared = shared;
        }

        public override string Name { get; }

        public override string Path => _shared ? AppPaths.RelativePathShared : AppPaths.RelativePath;

        public override string PhysicalPath => (_shared ? AppPaths.PhysicalPathShared : AppPaths.PhysicalPath);

        public override string Url => _shared ? AppPaths.PathShared : AppPaths.Path;


    }
}
