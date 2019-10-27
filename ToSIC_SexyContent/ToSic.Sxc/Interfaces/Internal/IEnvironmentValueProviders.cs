using ToSic.Eav.ValueProvider;

namespace ToSic.Sxc.Interfaces
{
    internal interface IEnvironmentValueProviders
    {
        ValueCollectionProvider GetProviders(int instanceId);
    }
}