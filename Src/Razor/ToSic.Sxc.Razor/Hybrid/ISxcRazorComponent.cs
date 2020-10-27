using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Razor.Hybrid
{
    public interface ISxcRazorComponent
    {
        DynamicCodeRoot DynCode { get; set; }

        //string VirtualPath { get; set; }

        Purpose Purpose { get; set; }

    }
}
