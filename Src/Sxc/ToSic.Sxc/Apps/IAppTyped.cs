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
        Eav.Apps.IApp,
        IAppPaths
    {
        /// <summary>
        /// Configuration object as a DynamicEntity.
        /// This contains things like app version, path etc.
        /// </summary>
        /// <returns>An <see cref="IDynamicEntity"/> object</returns>
        AppConfiguration Configuration { get; }

        /// <summary>
        /// All the app settings which are custom for each app. 
        /// </summary>
        /// <returns>An <see cref="IDynamicEntity"/> object</returns>
        ITypedItem Settings { get;  }

        /// <summary>
        /// All the app resources (usually used for multi-language labels etc.)
        /// </summary>
        /// <returns>An <see cref="IDynamicEntity"/> object</returns>
        ITypedItem Resources { get;  }

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
        string Thumbnail { get; }



        // TODO: @2dm - must create an ITypedAppData - create a new wrapper object and use it for MyData as well
        // Also rename IAppTyped to ITypedApp?

        ///// <inheritdoc cref="IMyData.Item"/>
        //ITypedItem Item(
        //    string streamName = default,
        //    string noParamOrder = Protector,
        //    ITypedItem fallback = default,
        //    bool? required = default
        //);

        ///// <inheritdoc cref="IMyData.Items"/>
        //IEnumerable<ITypedItem> Items(
        //    string streamName = default,
        //    string noParamOrder = Protector,
        //    IEnumerable<ITypedItem> fallback = default,
        //    bool? required = default,
        //    bool? preferNull = default);

        ///// <inheritdoc cref="IMyData.DataSource"/>
        //IDataSource DataSource { get; }
    }
}
