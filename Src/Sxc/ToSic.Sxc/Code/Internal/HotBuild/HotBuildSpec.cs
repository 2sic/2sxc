using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HotBuildSpec(int appId, string edition, string appName)
{
    public int AppId { get; } = appId;

    public string Edition { get; } = edition;

    public string AppName => appName;

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() => _toString ??= $"{nameof(HotBuildSpec)} - {nameof(AppId)}: {AppId} {(appName.HasValue() ? $"({appName})" : "")}; {nameof(Edition)}: '/{Edition}'";
    private string _toString;

    /// <summary>
    /// Create a dictionary of the specs for logging
    /// </summary>
    public IDictionary<string, string> ToDictionary() => new Dictionary<string, string>
    {
        { nameof(AppId), AppId.ToString() },
        { nameof(AppName), AppName ?? ""},
        { nameof(Edition), $"/{Edition}" },
    };

    public HotBuildSpec CloneWithoutEdition() => new(AppId, null, appName);
}