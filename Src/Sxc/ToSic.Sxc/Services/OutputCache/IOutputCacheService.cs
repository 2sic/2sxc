namespace ToSic.Sxc.Services.OutputCache;

internal interface IOutputCacheService
{
    int ModuleId { get; set; }
    string Disable();
    string Configure(OutputCacheSettings settings);
    string Enable(bool enable = true);
}