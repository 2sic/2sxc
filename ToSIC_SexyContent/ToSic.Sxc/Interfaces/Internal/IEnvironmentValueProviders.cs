
using ToSic.Eav.ValueProviders;

namespace ToSic.Sxc.Interfaces
{
    internal interface IEnvironmentValueProviders
    {
        ValueCollectionProvider GetProviders(int instanceId);
    }
}