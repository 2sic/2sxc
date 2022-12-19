using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("not sure where/if it goes anywhere")]
    public interface IRazor: IHasDynamicCodeRoot, INeedsDynamicCodeRoot
    {
    }
}
