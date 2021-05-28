using ToSic.Eav.Data;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Defines a view configuration which is loaded from an <see cref="EntityBasedType"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface IView: IEntityBasedType
    {
        /// <summary>
        /// The name, localized in the current UI language.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// An optional unique identifier for this View configuration. 
        /// </summary>
        /// <remarks>New in 12.02</remarks>
        string Identifier { get; }
        
        /// <summary>
        /// An optional Icon for this View configuration. Would be used instead of the file name in the App-folder. WIP
        /// </summary>
        /// <remarks>New in 12.02</remarks>
        string Icon { get; }
        
        /// <summary>
        /// Path to the template
        /// </summary>
        string Path { get; }

        [PrivateApi] string ContentType { get; }
        [PrivateApi] IEntity ContentItem { get; }
        [PrivateApi] string PresentationType { get; }
        [PrivateApi] IEntity PresentationItem { get; }
        [PrivateApi] string HeaderType { get; }
        [PrivateApi] IEntity HeaderItem { get; }
        [PrivateApi] string HeaderPresentationType { get; }
        [PrivateApi] IEntity HeaderPresentationItem { get; }

        /// <summary>
        /// The underlying type name of the template, ATM they are unfortunately hard-coded as "C# Razor" and "Token"
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Determine if we should hide this view/template from the pick-UI.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Location of the template - in the current tenant/portal or global/shared location.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Translates the location to tell us if it's a shared view (the template is in a shared location)
        /// </summary>
        bool IsShared { get; }

        /// <summary>
        /// Determines if the view should behave as a list or not. Views that are lists also
        /// have Header configuration and treat content in a special way. 
        /// </summary>
        [PrivateApi] bool UseForList { get; }

        [PrivateApi] bool PublishData { get; }
        [PrivateApi] string StreamsToPublish { get; }

        /// <summary>
        /// The query which provides data to this view. 
        /// </summary>
        [PrivateApi]
        IEntity QueryRaw { get; }

        /// <summary>
        /// The query attached to this view (if one was specified)
        /// </summary>
        /// <returns>A query object or null</returns>
        QueryDefinition Query { get; }

        /// <summary>
        /// An identifier which could occur in the url, causing the view to automatically switch to this one. 
        /// </summary>
        [PrivateApi] string UrlIdentifier { get; }

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        [PrivateApi]
        bool IsRazor { get; }

        /// <summary>
        /// Contains the polymorph edition name for this view, which changes
        /// what path is loaded.
        /// </summary>
        [PrivateApi]
        string Edition { get; set; }


        [PrivateApi("WIP 12.02")] IEntity Resources { get; }

        [PrivateApi("WIP 12.02")] IEntity Settings { get; }

    }
}