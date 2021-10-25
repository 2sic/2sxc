using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCode
    {
        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot?.CompatibilityLevel ?? 9;

    }
}
