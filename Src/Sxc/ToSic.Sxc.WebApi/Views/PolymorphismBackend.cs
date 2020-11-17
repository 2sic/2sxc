using System;
using ToSic.Eav.Apps;
using ToSic.Sxc.Polymorphism;

namespace ToSic.Sxc.WebApi.Views
{
    internal class PolymorphismBackend : WebApiBackendBase<PolymorphismBackend>
    {
        public PolymorphismBackend(IServiceProvider serviceProvider) : base(serviceProvider, "Bck.Views") { }




        public PolymorphismDto Polymorphism(int appId)
        {
            var callLog = Log.Call<dynamic>($"a#{appId}");
            var appState = State.Get(appId);
            var poly = new Polymorphism.Polymorphism(appState.List, Log);
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
