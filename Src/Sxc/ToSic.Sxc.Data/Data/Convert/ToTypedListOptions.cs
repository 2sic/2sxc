namespace ToSic.Sxc.Data.Convert;

// 2025-07-04 2dm - experimental ideas for more sophisticated options
// on Children<T> etc. to choose which children can be shown.
// Not in use yet, also not clear if we would have an object with all these settings/options
// or have more parameters in the command.

/// <summary>
/// WIP
/// </summary>
public record ToTypedListOptions
{
    public bool? AllowDrafts { get; set; }
}