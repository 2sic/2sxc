using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// View context information - this is Experimental / BETA WIP
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 12.02
    /// </remarks>
    [PublicApi]
    public interface ICmsView
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
        /// Get the views Metadata
        /// </summary>
        /// <remarks>
        /// Added in v12.10
        /// </remarks>
        IMetadataOf Metadata { get; }
    }
}
