namespace ToSic.Sxc.Data.Convert;

// 2025-07-04 2dm - experimental ideas for more sophisticated options
// on Children<T> etc. to choose which children can be shown.
// Not in use yet, also not clear if we would have an object with all these settings/options
// or have more parameters in the command.

/// <summary>
/// WIP
/// </summary>
public enum HandleNullOptions
{
    /// <summary>
    /// Auto means that nulls will be filtered out by default, so the list doesn't contain any null-values.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Skip means that nulls will be filtered out by default, so the list doesn't contain any null-values (same as Auto for now)
    /// </summary>
    Skip = 1,

    /// <summary>
    /// Preserve means that a null entry will be preserved as a null in the returned list.
    /// </summary>
    Preserve = 2,

    /// <summary>
    /// Wrap means that a null value will be given to the typed item / custom item, so that it is wrapped.
    /// </summary>
    /// <remarks>
    /// This is rarely a good idea, unless the object itself has special null-handling logic.
    /// </remarks>
    Wrap = 3,
}