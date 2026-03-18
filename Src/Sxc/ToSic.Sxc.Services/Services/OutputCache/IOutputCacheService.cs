namespace ToSic.Sxc.Services.OutputCache;

public interface IOutputCacheService
{
    int ModuleId { get; set; }
    string Disable();
    string Configure(OutputCacheSettings settings);
    string DependOn(string key);
    string Enable(bool enable = true);
}
