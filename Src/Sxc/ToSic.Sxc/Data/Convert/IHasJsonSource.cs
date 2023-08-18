using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public interface IHasJsonSource
    {
        object JsonSource { get; }
    }
}
