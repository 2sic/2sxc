using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Client.Services;

public interface IOqtDebugStateService
{
    bool IsDebugEnabled { get; }
    Task<bool> GetDebugAsync();
    void SetDebug(bool value);
}