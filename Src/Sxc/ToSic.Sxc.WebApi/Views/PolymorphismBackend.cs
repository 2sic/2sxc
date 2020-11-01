using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Apps;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Polymorphism;

namespace ToSic.Sxc.WebApi.Views
{
    internal class PolymorphismBackend : WebApiBackendBase<PolymorphismBackend>
    {
        public PolymorphismBackend() : base("Bck.Views") { }




        public PolymorphismDto Polymorphism(int appId)
        {
            var callLog = Log.Call<dynamic>($"a#{appId}");
            var cms = new CmsRuntime(State.Identity(null, appId), Log, true, false);
            var poly = new Polymorphism.Polymorphism(cms.Data, Log);
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
