using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    internal interface IHasJsonSource
    {
        object JsonSource { get; }
    }
}
