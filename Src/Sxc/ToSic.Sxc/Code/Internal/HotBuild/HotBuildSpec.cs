using System.IO;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HotBuildSpec
{
    public int AppId { get; set; }

    public string Edition { get; set; }

    public HotBuildEnum Segment { get; set; } = HotBuildEnum.Code;

    public bool HasThisAppSegmentInEdition { get; private set; }
    public void SetHasThisAppInEdition(string appRootPath)
    {
        if (string.IsNullOrEmpty(Edition))
        {
            HasThisAppSegmentInEdition = false;
            return;
        }

        // build expected path to ThisApp Segment folder in Edition
        var thisAppInEditionPath = Path.Combine(appRootPath, Edition, ThisAppCodeLoader.ThisAppCodeBase, Segment.ToString());

        // check do we have ThisApp Segment folder in Edition
        HasThisAppSegmentInEdition = Directory.Exists(thisAppInEditionPath);

        // check is there any files in folder thisAppInEditionPath
        if (HasThisAppSegmentInEdition)
        {
            var files = Directory.GetFiles(thisAppInEditionPath);
            HasThisAppSegmentInEdition = files.Length > 0;
        }
    }

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() => $"{nameof(HotBuildSpec)} - {nameof(AppId)}: {AppId}; {nameof(Edition)}: '{Edition}'; {nameof(Segment)}: '{Segment}'; {nameof(HasThisAppSegmentInEdition)}: '{HasThisAppSegmentInEdition}'";

    /// <summary>
    /// Create a dictionary of the specs for logging
    /// </summary>
    public IDictionary<string, string> ToDictionary() => new Dictionary<string, string>
    {
        { "AppId", AppId.ToString() },
        { "Edition", Edition },
        { "Segment", Segment.ToString() },
        { "HasThisAppSegmentInEdition", HasThisAppSegmentInEdition.ToString() }
    };
}