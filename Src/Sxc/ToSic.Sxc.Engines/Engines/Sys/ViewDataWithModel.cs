namespace ToSic.Sxc.Engines.Sys;

/// <summary>
/// Contains data for the view, including the model.
/// </summary>
/// <remarks>
/// Additional data ATM is meant for passing back information after running the view.
/// Specific example ATM is caching information, which will be used to decide if the result should be cached or not.
/// 
/// This is currently done this way because we're not sure how else to get this information back to the engine,
/// because the Razor View engine is a bit internal and can't just change the return signature.
/// </remarks>
public record ViewDataWithModel
{
    public object? Data { get; init; }

    public bool AlwaysCache { get; set; }
}