using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IRazorService: INeedsDynamicCodeRoot
{
    string Render(string partialName, object model = null);
}