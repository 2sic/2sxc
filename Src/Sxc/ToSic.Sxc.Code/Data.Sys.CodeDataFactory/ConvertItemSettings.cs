namespace ToSic.Sxc.Data.Sys.CodeDataFactory;
public record ConvertItemSettings
{
    public required bool ItemIsStrict { get; init; }
    public required bool DropNullItems { get; init; }
}
