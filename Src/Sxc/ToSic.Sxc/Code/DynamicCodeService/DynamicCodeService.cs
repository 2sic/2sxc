using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
    /// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
    /// </summary>
    public class DynamicCodeService: HasLog, IDynamicCodeService
    {
        public DynamicCodeService(IServiceProvider serviceProvider): base("WIP.DCS")
        {
            _entryServiceProvider = serviceProvider;
            _serviceScope = serviceProvider.CreateScope();
            _serviceProvider = _serviceScope.ServiceProvider;
        }
        private readonly IServiceProvider _entryServiceProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;

        public IDynamicCode OfApp(int appId)
        {
            var ctxResolver = _serviceProvider.Build<IContextResolver>();
            var appCtx = ctxResolver.App(appId);
            var dynCodeRoot = _serviceProvider.Build<DynamicCodeRoot>();
            // dynCodeRoot.Init()
            return null;
        }

        public IDynamicCode OfModule(int moduleId) => null;

        public IDynamicCode OfModule(int pageId, int moduleId) => null;
    }
}
