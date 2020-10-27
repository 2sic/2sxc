using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Hybrid.Razor
{
    public interface ISxcRazorComponent
    {
        DynamicCodeRoot DynCode { get; set; }

        Purpose Purpose { get; set; }

    }
}
