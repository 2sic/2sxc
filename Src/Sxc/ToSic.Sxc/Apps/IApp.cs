using ToSic.Eav.Apps.Paths;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;
#if !NETFRAMEWORK
#pragma warning disable CS0109
#endif
// ReSharper disable UnusedMemberInSuper.Global

namespace ToSic.Sxc.Apps
{
    /// <summary>
    /// An app-object as is available in a razor template or WebApi
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface IApp: 
        Eav.Apps.IApp,
        IAppPaths
#if NETFRAMEWORK
        , SexyContent.Interfaces.IApp // inherits from old namespace for compatibility
#endif
    {
        /// <summary>
        /// Configuration object as a DynamicEntity.
        /// This contains things like app version, path etc.
        /// </summary>
        /// <returns>An <see cref="IDynamicEntity"/> object</returns>
        new AppConfiguration Configuration { get; }

        /// <summary>
        /// All the app settings which are custom for each app. 
        /// </summary>
        /// <returns>An <see cref="IDynamicEntity"/> object</returns>
        new dynamic Settings { get;  }

        /// <summary>
        /// All the app resources (usually used for multi-language labels etc.)
        /// </summary>
        /// <returns>An <see cref="IDynamicEntity"/> object</returns>
        new dynamic Resources { get;  }

        /// <summary>
        /// The path to the current app, for linking JS/CSS files and
        /// images in the app folder. 
        /// </summary>
        /// <returns>Path usually starting with /portals/...</returns>
        new string Path { get; }

        /// <summary>
        /// The path on the server hard disk for the current app. 
        /// </summary>
        /// <returns>Path usually starting with c:\...</returns>
        new string PhysicalPath { get; }

        /// <summary>
        /// The path to the current apps shared/global folder, for linking JS/CSS files and
        /// images in the app folder. 
        /// </summary>
        /// <returns>Path usually starting with /portals/_default/...</returns>
        /// <remarks>Added v13.01</remarks>
#pragma warning disable CS0108, CS0114
        // Important: Repeat definition of base interface for docs and because of Razor-Interface-Inheritance-Problems
        string PathShared { get; }
#pragma warning restore CS0108, CS0114

        /// <summary>
        /// The path on the server hard disk for the current apps shared/global folder. 
        /// </summary>
        /// <returns>Path usually starting with c:\...</returns>
        /// <remarks>Added v13.01</remarks>
#pragma warning disable CS0108, CS0114
        // Important: Repeat definition of base interface for docs and because of Razor-Interface-Inheritance-Problems
        string PhysicalPathShared { get; }
#pragma warning restore CS0108, CS0114

        /// <summary>
        /// The thumbnail path for the current app. 
        /// </summary>
        /// <returns>path + app-icon.png if there is an icon there. </returns>
        new string Thumbnail { get; }

        //[PrivateApi("not public, not sure if we should surface this")]
        //string RelativePath { get; }

        //[PrivateApi("not public, not sure if we should surface this")]
        //string RelativePathShared { get; }
    }
}
