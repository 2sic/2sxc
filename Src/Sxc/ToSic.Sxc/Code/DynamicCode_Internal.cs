using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCode
    {
        [PrivateApi] public int CompatibilityLevel => UnwrappedContents?.CompatibilityLevel ?? 9;

        //[PrivateApi] public IBlock _Block => UnwrappedContents?._Block;

    }
}
