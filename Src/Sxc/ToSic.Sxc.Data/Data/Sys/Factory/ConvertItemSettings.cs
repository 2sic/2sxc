namespace ToSic.Sxc.Data.Sys.Factory;
public record ConvertItemSettings
{
    /// <summary>
    /// Accessing the property at the entry level (top of the tree, not sub-properties) is required, the property must exist.
    /// </summary>
    public bool EntryPropIsRequired { get; init; } = true;
    public required bool ItemIsStrict { get; init; }
    public bool UseMock { get; init; } = false;
    public bool DropNullItems { get; init; } = true;
}
