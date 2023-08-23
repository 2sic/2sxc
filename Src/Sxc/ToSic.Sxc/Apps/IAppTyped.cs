using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Data;

// ReSharper disable UnusedMemberInSuper.Global

namespace ToSic.Sxc.Apps
{
    /// <summary>
    /// An app-object as is available in a razor template or WebApi
    /// </summary>
    [PublicApi]
    public interface IAppTyped: 
        Eav.Apps.IApp
        //IAppPaths
    {
        /// <inheritdoc cref="IApp.Configuration"/>
        AppConfiguration Configuration { get; }

        /// <summary>
        /// All the app settings which are custom for each app. 
        /// </summary>
        ITypedItem Settings { get;  }

        /// <summary>
        /// All the app resources (usually used for multi-language labels etc.)
        /// </summary>
        ITypedItem Resources { get;  }

        #region Paths / Urls

        /// <summary>
        /// Retrieve the url to the location of the files of this App.
        /// This replaces the previous `Path` and `PathShared` properties, but with some improvements.
        /// It will automatically detect if the App files are located in the site or shared, and return that url.
        /// 
        /// If you need more control, use <see cref="UrlAdvanced"/>
        /// </summary>
        /// <returns></returns>
        /// <remarks>Added v16.04</remarks>
        string Url { get; }

        /// <summary>
        /// Retrieve the url to the location of the files of this App.
        ///
        /// 1. In the standard setup, it's just the path to the app files in the current site
        /// 2. If the app is shared - meaning that the files are in a shared location, it will return that path
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="location">name of the app location - either `auto` (default), `site` or `shared`</param>
        /// <returns></returns>
        /// <remarks>Added v16.04</remarks>
        string UrlAdvanced(string noParamOrder = Eav.Parameters.Protector, string location = default);

        ///// <summary>
        ///// The path to the current app, for linking JS/CSS files and
        ///// images in the app folder. 
        ///// </summary>
        ///// <returns>Path usually starting with /portals/...</returns>
        //new string Path { get; }

        //        /// <summary>
        //        /// The path to the current apps shared/global folder, for linking JS/CSS files and
        //        /// images in the app folder. 
        //        /// </summary>
        //        /// <returns>Path usually starting with /portals/_default/...</returns>
        //        /// <remarks>Added v13.01</remarks>
        //#pragma warning disable CS0108, CS0114
        //        // Important: Repeat definition of base interface for docs and because of Razor-Interface-Inheritance-Problems
        //        string PathShared { get; }
        //#pragma warning restore CS0108, CS0114

        /// <summary>
        /// The path on the server hard disk for the current app. 
        /// </summary>
        /// <returns>Path usually starting with c:\...</returns>
        new string PhysicalPath { get; }



        /// <summary>
        /// The path on the server hard disk for the current apps shared/global folder. 
        /// </summary>
        /// <returns>Path usually starting with c:\...</returns>
        /// <remarks>Added v13.01</remarks>
#pragma warning disable CS0108, CS0114
        // Important: Repeat definition of base interface for docs and because of Razor-Interface-Inheritance-Problems
        string PhysicalPathShared { get; }
#pragma warning restore CS0108, CS0114

        #endregion



        /// <inheritdoc cref="IApp.Thumbnail"/>
        string Thumbnail { get; }


    }
}
