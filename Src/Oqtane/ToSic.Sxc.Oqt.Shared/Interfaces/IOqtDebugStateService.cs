using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

public interface IOqtDebugStateService
{
    bool IsDebugEnabled { get; }
    Task<bool> GetDebugAsync();
    void SetDebug(bool value);
}