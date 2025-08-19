using System.Diagnostics;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// Helper to track the total time spent on each partial, especially when a partial is used many, many times (like in lists).
/// </summary>
internal class HtmlHelperTimeKeeper
{
    public Dictionary<string, Stopwatch> Partials = new();

    public Stopwatch Start(string partialName)
    {
        if (!Partials.TryGetValue(partialName, out var sw))
            return Partials[partialName] = Stopwatch.StartNew();
        
        sw.Start();
        return sw;
    }
}
