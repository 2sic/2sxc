using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web
{
    public interface IRazorService: INeedsCodeRoot
    {
        string Render(string partialName, object model = null);
    }
}