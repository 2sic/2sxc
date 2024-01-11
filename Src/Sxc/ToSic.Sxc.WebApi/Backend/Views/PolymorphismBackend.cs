using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Backend.Views;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphismBackend : ServiceBase
{
    public PolymorphismBackend(Polymorphism.Internal.PolymorphConfigReader polymorphism, IAppStates appStates) : base("Bck.Views")
    {
        ConnectServices(
            _polymorphism = polymorphism,
            _appStates = appStates
        );
    }

    private readonly Polymorphism.Internal.PolymorphConfigReader _polymorphism;
    private readonly IAppStates _appStates;

    public PolymorphismDto Polymorphism(int appId)
    {
        var callLog = Log.Fn<PolymorphismDto>($"a#{appId}");
        var appState = _appStates.GetReader(appId);
        var poly = _polymorphism.Init(appState.List);
        var result = new PolymorphismDto
        {
            Id = poly.Entity?.EntityId, 
            Resolver = poly.Resolver, 
            TypeName = PolymorphismConstants.Name
        };
        return callLog.Return(result);
    }
}