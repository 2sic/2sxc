using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Backend.Views;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphismBackend(PolymorphConfigReader polymorphism, IAppStates appStates)
    : ServiceBase("Bck.Views", connect: [polymorphism, appStates])
{
    public PolymorphismDto Polymorphism(int appId)
    {
        var callLog = Log.Fn<PolymorphismDto>($"a#{appId}");
        var appState = appStates.GetReader(appId);
        var poly = polymorphism.Init(appState.List);
        var result = new PolymorphismDto
        {
            Id = poly.Configuration.Id, 
            Resolver = poly.Resolver, 
            TypeName = PolymorphismConfiguration.Name
        };
        return callLog.Return(result);
    }
}