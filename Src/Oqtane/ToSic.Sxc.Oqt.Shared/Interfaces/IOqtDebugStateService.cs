using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOqtDebugStateService
{
    bool IsDebugEnabled { get; }
    Task<bool> GetDebugAsync();
    void SetDebug(bool value);
    string Platform { get; }
}