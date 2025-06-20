namespace ToSic.Sxc.Data.CodeData.Internal;
public record ConvertItemSettings
{
    public required bool ItemIsStrict { get; init; }
    public required bool DropNullItems { get; init; }
}
