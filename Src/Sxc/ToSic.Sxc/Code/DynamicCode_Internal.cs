using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCode
    {
        [PrivateApi] public int CompatibilityLevel => UnwrappedContents?.CompatibilityLevel ?? 9;

        [PrivateApi] public IBlock Block => UnwrappedContents?.Block;


        //public IContextOfSite Context => ((IDynamicCodeInternal)UnwrappedContents).Context;
    }
}
