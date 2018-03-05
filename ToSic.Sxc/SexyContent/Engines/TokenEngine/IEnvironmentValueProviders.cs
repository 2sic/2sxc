using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.SexyContent.Engines.TokenEngine
{
    public interface IEnvironmentValueProviders
    {
        ValueCollectionProvider GetProviders(int instanceId);
    }
}