using ToSic.Eav.DataSource.Internal.Query;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// Defines a view configuration which is loaded from a <see cref="EntityBasedType"/>.
/// </summary>
[PrivateApi("Was Public API till v17, but I can't see any reason why people would have used it since it would go through ICmsView")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
    /// Translates the location to tell us if it's a shared view (the template is in a shared location)
    /// </summary>
    // TODO: SHOULD PROBABLY rename to something else like IsGlobal ? 
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
    [PrivateApi] string Edition { get; set; }

    [PrivateApi] string EditionPath { get; set; }


    [PrivateApi("WIP 12.02")] IEntity Resources { get; }

    [PrivateApi("WIP 12.02")] IEntity Settings { get; }

    /// <summary>
    /// Determines if search indexing should be disabled - so this view will not provide search data.
    /// </summary>
    bool SearchIndexingDisabled { get; }
        
    /// <summary>
    /// The external class which should be compiled / used to customize search.
    /// 
    /// In future this could do more, which is why it's called ViewController and not SearchController or something. 
    /// </summary>
    string ViewController { get; }

    /// <summary>
    /// Streams which should be included in the search index.
    /// If empty will use all streams.
    /// CSV
    /// </summary>
    string SearchIndexingStreams { get; }

    /// <summary>
    /// Inform the system that this view was replaced, e.g. because of the url-parameter
    /// </summary>
    [PrivateApi]
    bool IsReplaced { get; }
}