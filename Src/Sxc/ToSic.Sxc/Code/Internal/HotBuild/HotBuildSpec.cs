namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HotBuildSpec(int appId, string edition = default, HotBuildEnum segment = HotBuildEnum.Code)
{
    public int AppId { get; private set; } = appId;

    public string Edition { get; private set; } = edition;

    public HotBuildEnum Segment { get; private set; } = segment;

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() => $"{nameof(HotBuildSpec)} - {nameof(AppId)}: {AppId}; {nameof(Edition)}: '{Edition}'; {nameof(Segment)}: '{Segment}'";

    /// <summary>
    /// Create a dictionary of the specs for logging
    /// </summary>
    public IDictionary<string, string> ToDictionary() => new Dictionary<string, string>
    {
        { "AppId", AppId.ToString() },
        { "Edition", Edition },
        { "Segment", Segment.ToString() },
    };

    public HotBuildSpec CloneWithoutEdition() => new(AppId, null, Segment);
}