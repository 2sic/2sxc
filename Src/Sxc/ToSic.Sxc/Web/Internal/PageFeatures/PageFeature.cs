using ToSic.Eav.SysData;

namespace ToSic.Sxc.Web.Internal.PageFeatures;

/// <summary>
/// A feature describes something that can be enabled on a page. It can be a script, some css, an inline JS or combinations thereof.
/// This is just the information which is prepared. It will be in a list of features to activate.
/// </summary>
[PrivateApi("Internal / not final - neither name, namespace or anything")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public record PageFeature : IPageFeature
{
    public const string ConditionIsPageFeature = "pagefeature";
        
    /// <summary>
    /// Primary identifier to activate the feature
    /// </summary>
    public required string NameId { get; init; }

    /// <summary>
    /// Name of this feature. 
    /// </summary>
    public string Name { get; init; } = "";

    public string Html { get; set; } // TODO: INIT!

    /// <summary>
    /// Nice description of the feature.
    /// </summary>
    public string Description { get; init; } = "";

    /// <summary>
    /// List of other features required to run this feature.
    /// </summary>
    public IEnumerable<string> Needs { get; init; } = [];

    public Requirement Requirement => field ??= new(ConditionIsPageFeature, NameId);

    public List<Requirement> Requirements { get; init; } = [];

    /// <summary>
    /// Temporary URL for internal features which need to store the URL someplace
    /// This is not a final solution, in future it should probably
    /// be more sophisticated, like contain a list of configuration objects to construct the url.
    /// </summary>
    public string UrlInDist { get; init; }

    /// <summary>
    /// ToString for easier debugging
    /// </summary>
    public override string ToString()
        => $"{base.ToString()}({NameId})";
}