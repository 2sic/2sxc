using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Apps.Paths
{
    /// <summary>
    /// Internal interface to enable helpers to switch between paths on both the App and AppPaths
    /// </summary>
    [PrivateApi]
    public interface IAppPaths
    {
        /// <summary>
        /// The path to the current app, for linking JS/CSS files and
        /// images in the app folder. 
        /// </summary>
        /// <returns>Path usually starting with /portals/...</returns>
        string Path { get; }

        /// <summary>
        /// The path on the server hard disk for the current app. 
        /// </summary>
        /// <returns>Path usually starting with c:\...</returns>
        string PhysicalPath { get; }

        /// <summary>
        /// The path to the current app global folder, for linking JS/CSS files and
        /// images in the app folder. 
        /// </summary>
        /// <returns>Path usually starting with /portals/_default/...</returns>
        string PathShared { get; }

        /// <summary>
        /// The path on the server hard disk for the current app global folder. 
        /// </summary>
        /// <returns>Path usually starting with c:\...</returns>
        string PhysicalPathShared { get; }

        [PrivateApi("not public, not sure if we should surface this")]
        string RelativePath { get; }

        [PrivateApi("not public, not sure if we should surface this")]
        string RelativePathShared { get; }
    }
}
