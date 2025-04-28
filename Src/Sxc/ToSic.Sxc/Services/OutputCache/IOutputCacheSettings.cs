namespace ToSic.Sxc.Services.OutputCache;

/// <summary>
/// This interface is internal, to just ensure that all classes use the same property names.
/// ATM it's not meant for any kind of public use.
/// </summary>
internal interface IOutputCacheSettings
{
    bool IsEnabled { get; }
    int Duration { get; }
    int DurationUsers { get; }
    int DurationEditors { get; }
    int DurationSystemAdmin { get; }
    bool ByUrlParameters { get; }
    bool UrlParametersCaseSensitive { get; }
    string UrlParameterNames { get; }
    bool UrlParametersOthersDisableCache { get; }
}