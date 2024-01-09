using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Razor.Internal;

// 2024-01-09 2dm seems unused
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IRazorService: INeedsDynamicCodeRoot
{
    string Render(string partialName, object model = null);
}