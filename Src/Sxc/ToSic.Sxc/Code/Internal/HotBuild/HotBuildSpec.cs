using System.Collections;
using System.Collections.Generic;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HotBuildSpec
{
    public int AppId { get; set; }

    public string Edition { get; set; }

    public HotBuildEnum Segment { get; set; } = HotBuildEnum.Code;

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
}