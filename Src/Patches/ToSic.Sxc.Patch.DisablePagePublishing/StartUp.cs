using System.Linq;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Patch.DisablePagePublishing
{
    public class StartUp: IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            // Stop if this had already run, and don't re-run no matter what happens afterwards
            if (_alreadyRegistered) return;
            _alreadyRegistered = true;

            var found = false;
            Factory.ActivateNetCoreDi(services =>
            {
                // Use Try-Add in case this code runs before the DNN standard initialization
                // It will be skipped if Dnn already registered it's resolver
                var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IPagePublishingResolver));
                found = serviceDescriptor != null;
                if (found)
                    services.Remove(serviceDescriptor);
                services.TryAddTransient<IPagePublishingResolver, DisablePagePublishingResolver>();

                // Now replace existing. If Dnn had already run, it will replace that, otherwise the one we just added
                //services.Replace(new ServiceDescriptor(typeof(IPagePublishing), typeof(DisablePagePublishingResolver)));
            });

            // Place a note into the insights, so it's clear that this happened
            try
            {
                var log = new Log("Wrn.Disabl");
                log.Add("Disable PagePublishing enabled");
                log.Add("This is just an info for the insights so you can see that something custom was done to this DNN");
                log.Add($"Additional info: previous registration found = {found}");
                History.Add(Constants.PatchesHistoryName, log);
            }
            catch { /* ignore */ }
            
        }

        private bool _alreadyRegistered;
    }
}
