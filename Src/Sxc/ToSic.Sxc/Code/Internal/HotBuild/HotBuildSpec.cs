namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HotBuildSpec(int appId, string edition = default)
{
    public int AppId { get; } = appId;

    public string Edition { get; } = edition;

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() => _toString ??= $"{nameof(HotBuildSpec)} - {nameof(AppId)}: {AppId}; {nameof(Edition)}: '{Edition}'";
    private string _toString;

    /// <summary>
    /// Create a dictionary of the specs for logging
    /// </summary>
    public IDictionary<string, string> ToDictionary() => new Dictionary<string, string>
    {
        { "AppId", AppId.ToString() },
        { "Edition", Edition },
    };

    public HotBuildSpec CloneWithoutEdition() => new(AppId, null);
}