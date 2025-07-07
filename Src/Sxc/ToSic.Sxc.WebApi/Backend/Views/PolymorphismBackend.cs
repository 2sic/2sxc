using ToSic.Sxc.Polymorphism.Sys;

namespace ToSic.Sxc.Backend.Views;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class PolymorphismBackend(PolymorphConfigReader polymorphism, IAppReaderFactory appReaders)
    : ServiceBase("Bck.Views", connect: [polymorphism, appReaders])
{
    public PolymorphismDto Polymorphism(int appId)
    {
        var l = Log.Fn<PolymorphismDto>($"a#{appId}");
        var appState = appReaders.Get(appId);
        var poly = polymorphism.Init(appState.List);
        var result = new PolymorphismDto
        {
            Id = poly.Configuration.Id, 
            Resolver = poly.Configuration.Resolver, 
            TypeName = PolymorphismConfiguration.Name
        };
        return l.Return(result);
    }
}