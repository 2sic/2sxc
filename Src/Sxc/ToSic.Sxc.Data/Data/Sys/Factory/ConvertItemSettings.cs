namespace ToSic.Sxc.Data.Sys.Factory;
public record ConvertItemSettings
{
    public bool FirstIsRequired { get; init; } = true;
    public required bool ItemIsStrict { get; init; }
    public bool UseMock { get; init; } = false;
    public bool DropNullItems { get; init; } = true;
}
