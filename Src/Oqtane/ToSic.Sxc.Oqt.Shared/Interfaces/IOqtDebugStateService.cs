using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IOqtDebugStateService
{
    bool IsDebugEnabled { get; }
    Task<bool> GetDebugAsync();
    void SetDebug(bool value);
    string Platform { get; }
}