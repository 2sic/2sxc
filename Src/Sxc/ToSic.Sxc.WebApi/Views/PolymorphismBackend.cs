using System;
using ToSic.Eav.Apps;
using ToSic.Sxc.Polymorphism;

namespace ToSic.Sxc.WebApi.Views
{
    internal class PolymorphismBackend : WebApiBackendBase<PolymorphismBackend>
    {
        private readonly Polymorphism.Polymorphism _polymorphism;
        public PolymorphismBackend(IServiceProvider serviceProvider, Polymorphism.Polymorphism polymorphism) : base(serviceProvider, "Bck.Views")
        {
            _polymorphism = polymorphism;
        }

        
        public PolymorphismDto Polymorphism(int appId)
        {
            var callLog = Log.Call<dynamic>($"a#{appId}");
            var appState = State.Get(appId);
            var poly = _polymorphism.Init(appState.List, Log); // new Polymorphism.Polymorphism(appState.List, Log);
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
