using ToSic.Eav.Apps;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Polymorphism;

namespace ToSic.Sxc.WebApi.Views
{
    public class PolymorphismBackend : ServiceBase
    {
        public PolymorphismBackend(Polymorphism.Polymorphism polymorphism, IAppStates appStates) : base("Bck.Views")
        {
            _polymorphism = polymorphism;
            _appStates = appStates;
        }

        private readonly Polymorphism.Polymorphism _polymorphism;
        private readonly IAppStates _appStates;

        public PolymorphismDto Polymorphism(int appId)
        {
            var callLog = Log.Fn<dynamic>($"a#{appId}");
            var appState = _appStates.Get(appId);
            var poly = _polymorphism.Init(appState.List, Log);
            var result = new PolymorphismDto
            {
                Id = poly.Entity?.EntityId, 
                Resolver = poly.Resolver, 
                TypeName = PolymorphismConstants.Name
            };
            return callLog.Return(result);
        }
    }
}
