namespace ToSic.Sxc.Services.OutputCache;

// WIP v19.03.03
// Not complete. To finish:
// - Figure out how to best implement the logic for combining settings from different sources
// - eg. skip zeros when merging, merge booleans if not defined, etc.
// - also consider adding more dependencies (may mix with the Cache API) to e.g. create a dependency on a data-stream such as cached SharePoint data?
// - then put on Kit
// - then document

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record OutputCacheSettings : IOutputCacheSettings
{
    public bool IsEnabled { get; init; } = true;

    public int Duration { get; init; } = 0;

    public int DurationUsers { get; init; }

    public int DurationEditors { get; init; }

    public int DurationSystemAdmin { get; init; }

    public bool ByUrlParameters { get; init; }

    public bool UrlParametersCaseSensitive { get; init; }

    public string? UrlParameterNames { get; init; }

    public bool UrlParametersOthersDisableCache { get; init; }

    public IReadOnlyCollection<string>? ExternalDependencyKeys { get; init; }
}
