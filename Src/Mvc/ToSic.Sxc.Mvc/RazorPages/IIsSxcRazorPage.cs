using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Mvc.RazorPages
{
    public interface IIsSxcRazorPage
    {
        DynamicCodeRoot DynCode { get; set; }

        string VirtualPath { get; set; }

        Purpose Purpose { get; set; }

    }
}
