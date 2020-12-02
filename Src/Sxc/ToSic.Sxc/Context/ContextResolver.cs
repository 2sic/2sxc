using System;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Context
{
    public class ContextResolver: HasLog<IContextResolver>, IContextResolver
    {
        private IServiceProvider ServiceProvider { get; }

        public ContextResolver(IServiceProvider serviceProvider) : base("Sxc.CtxRes")
        {
            ServiceProvider = serviceProvider;
        }

        public IContextOfSite Site() => ServiceProvider.Build<IContextOfSite>();

        public IContextOfApp App(int appId)
        {
            var appContext = ServiceProvider.Build<IContextOfApp>();
            appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }

        public IContextOfApp App(string appPathOrName)
        {
            throw new NotImplementedException();
        }
    }
}
