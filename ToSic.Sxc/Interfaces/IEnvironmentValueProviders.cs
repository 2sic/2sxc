using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.Interfaces
{
    internal interface IEnvironmentValueProviders
    {
        ValueCollectionProvider GetProviders(int instanceId);
    }
}