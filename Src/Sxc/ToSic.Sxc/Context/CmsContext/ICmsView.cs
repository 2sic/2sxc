using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// View context information - this is Experimental / BETA WIP
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 12.02
    /// </remarks>
    [PublicApi]
    public interface ICmsView: IHasMetadata
    {
        /// <summary>
        /// View configuration ID
        /// 
        /// 🪒 Use in Razor: `CmsContext.View.Id`
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Name of the view as configured - note that because of i18n it could be different depending on the language.
        /// To clearly identify a view, use the <see cref="Identifier"/> or use `Settings`
        /// 
        /// 🪒 Use in Razor: `CmsContext.View.Name`
        /// </summary>
        string Name { get; }

        /// <summary>
        /// An optional identifier which the View configuration can provide.
        /// Use this when you want to use the same template but make minor changes based on the View selected (like change the number of columns).
        /// Usually you will use either this OR the `Settings`
        /// 
        /// 🪒 Use in Razor: `CmsContext.View.Identifier`
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Edition used - if any. Otherwise empty string. 
        /// 
        /// 🪒 Use in Razor: `CmsContext.View.Edition`
        /// </summary>
        string Edition { get; }

        /// <summary>
        /// Metadata of the current view
        /// </summary>
        /// <remarks>
        /// Added in v13.12
        /// </remarks>
        // actually Added in v12.10; changed from IMetadataOf to IDynamicMetadata in 13.12
#pragma warning disable CS0108, CS0114
        IDynamicMetadata Metadata { get; }
#pragma warning restore CS0108, CS0114

        /// <summary>
        /// The path to this view.
        /// For URLs which should load js/css from a path beneath the view.
        ///
        /// This is different from the `App.Path`, because it will also contain the edition (if there is an edition)
        /// </summary>
        /// <remarks>
        /// Added in v15.04
        /// </remarks>
        string Path { get; }

        /// <summary>
        /// The path to this view in the global/shared location.
        /// For URLs which should load js/css from a path beneath the view.
        ///
        /// This is different from the `App.Path`, because it will also contain the edition (if there is an edition)
        /// </summary>
        /// <remarks>
        /// Added in v15.04
        /// </remarks>
        string PathShared { get; }

        /// <summary>
        /// The folder of view.
        /// For retrieving files on the server in the same path or below this. 
        ///
        /// This is different from the `App.PhysicalPath`, because it will also contain the edition (if there is an edition)
        /// </summary>
        /// <remarks>
        /// Added in v15.04
        /// </remarks>
        string PhysicalPath { get; }

        /// <summary>
        /// The folder of view in the global shared location.
        /// For retrieving files on the server in the same path or below this. 
        ///
        /// This is different from the `App.PhysicalPath`, because it will also contain the edition (if there is an edition)
        /// </summary>
        /// <remarks>
        /// Added in v15.04
        /// </remarks>
        string PhysicalPathShared { get; }
    }
}
