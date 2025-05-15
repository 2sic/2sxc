namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Just a helper to ensure that only known state names are used.
/// </summary>
/// <remarks>
/// This is for the <see cref="IExecutionContext.GetState{T}(string)"/>
/// </remarks>
public class ExecutionContextStateNames
{
    /// <summary>
    /// Settings must be a <see cref="IDynamicStack"/>
    /// </summary>
    public const string Settings = "Settings";

    /// <summary>
    /// AllSettings must be a <see cref="ITypedStack"/>
    /// </summary>
    public const string AllSettings = "AllSettings";

    /// <summary>
    /// AllResources must be a <see cref="ITypedStack"/>
    /// </summary>
    public const string AllResources = "AllResources";
}
