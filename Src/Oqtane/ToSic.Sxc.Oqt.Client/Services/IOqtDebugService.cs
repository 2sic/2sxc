namespace ToSic.Sxc.Oqt.Client.Services;

public interface IOqtDebugService
{
    bool Debug { get; set; } // persist state across circuits (blazor server only)
}