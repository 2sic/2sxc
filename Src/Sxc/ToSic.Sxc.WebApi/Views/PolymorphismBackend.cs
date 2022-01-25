using System;
using ToSic.Eav.Apps;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Polymorphism;

namespace ToSic.Sxc.WebApi.Views
{
    internal class PolymorphismBackend : WebApiBackendBase<PolymorphismBackend>
    {
        public PolymorphismBackend(IServiceProvider serviceProvider, Polymorphism.Polymorphism polymorphism, IAppStates appStates) : base(serviceProvider, "Bck.Views")
        {
            _polymorphism = polymorphism;
            _appStates = appStates;
        }

        private readonly Polymorphism.Polymorphism _polymorphism;
        private readonly IAppStates _appStates;

        public PolymorphismDto Polymorphism(int appId)
        {
            var callLog = Log.Call<dynamic>($"a#{appId}");
            var appState = _appStates.Get(appId);
            var poly = _polymorphism.Init(appState.List, Log);
            var result = new PolymorphismDto
            {
                Id = poly.Entity?.EntityId, 
                Resolver = poly.Resolver, 
                TypeName = PolymorphismConstants.Name
            };
            return callLog(null, result);
        }
    }
}
