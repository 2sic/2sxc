using ToSic.Eav.Work;

namespace ToSic.Sxc.DataSources;

public record AppAssetsGetSpecs: IWorkSpecs
{
    public int AppId { get; init; } = int.MinValue;

    public int ZoneId { get; init; } = int.MinValue;

    public string RootFolder { get; init; } = null;

    public string FileFilter { get; init; } = null;
}