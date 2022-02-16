using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web
{
    public interface IRazorService: INeedsDynamicCodeRoot
    {
        string Render(string partialName, object model = null);
    }
}