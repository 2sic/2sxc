using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Data;

// ReSharper disable UnusedMemberInSuper.Global

namespace ToSic.Sxc.Apps;

/// <summary>
/// An app-object as is available in a razor template or WebApi when in classic/dynamic mode.
/// </summary>
[PublicApi]
public interface IApp: 
    Eav.Apps.IApp,
    IAppPaths
#if NETFRAMEWORK
        , SexyContent.Interfaces.IApp // inherits from old namespace for compatibility
#endif
{
    /// <summary>
    /// Configuration object with information about the App.
    /// This contains things like app version, path etc.
    /// </summary>
#if NETFRAMEWORK
        new
#endif
    IAppConfiguration Configuration { get; }

    /// <summary>
    /// All the app settings which are custom for each app. 
    /// </summary>
    /// <returns>An <see cref="IDynamicEntity"/> object</returns>
#if NETFRAMEWORK
        new
#endif
    dynamic Settings { get;  }

    /// <summary>
    /// All the app resources (usually used for multi-language labels etc.)
    /// </summary>
    /// <returns>An <see cref="IDynamicEntity"/> object</returns>
#if NETFRAMEWORK
        new
#endif
    dynamic Resources { get;  }

    #region Query

    /// <summary>
    /// All queries of the app, to access like App.Query["name"]
    /// </summary>
    /// <returns>A dictionary with all queries. Internally the dictionary will not be built unless accessed.</returns>
    IDictionary<string, IQuery> Query { get; }

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IQuery GetQuery(string name);

    #endregion

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
// Important: Repeat definition of base interface for docs and because of Razor-Interface-Inheritance-Problems
    new string PathShared { get; }

    /// <summary>
    /// The path on the server hard disk for the current apps shared/global folder. 
    /// </summary>
    /// <returns>Path usually starting with c:\...</returns>
    /// <remarks>Added v13.01</remarks>
    // Important: Repeat definition of base interface for docs and because of Razor-Interface-Inheritance-Problems
    new string PhysicalPathShared { get; }

    ///// <summary>
    ///// Path relative to the website root.
    ///// In DNN this is usually the same as the url-path.
    ///// In Oqtane it's very different. 
    ///// </summary>
    ///// <remarks>
    ///// * Made public v15.06 but existed previously
    ///// </remarks>
    //[PrivateApi("not public, not sure if we should surface this")]
    //new string RelativePath { get; }

    ///// <summary>
    ///// Path of the shared App relative to the website root.
    ///// In DNN this is usually the same as the url-path.
    ///// In Oqtane it's very different. 
    ///// </summary>
    ///// <remarks>
    ///// * Made public v15.06 but existed previously
    ///// </remarks>
    //[PrivateApi("not public, not sure if we should surface this")]
    //new string RelativePathShared { get; }

    /// <summary>
    /// The thumbnail path for the current app. 
    /// </summary>
    /// <returns>path + app-icon.png if there is an icon there. </returns>
#if NETFRAMEWORK
        new
#endif
    string Thumbnail { get; }


}