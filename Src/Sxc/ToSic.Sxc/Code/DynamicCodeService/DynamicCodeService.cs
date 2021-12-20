using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
    /// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
    /// </summary>
    public class DynamicCodeService: IDynamicCodeService
    {
        public DynamicCodeService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serviceScope = serviceProvider.CreateScope();
        }
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;

        public IDynamicCode OfApp(int appId)
        {
            var ctxResolver = _serviceScope.ServiceProvider.Build<IContextResolver>();
            var appCtx = ctxResolver.App(appId);
            return null;
        }

        public IDynamicCode OfModule(int moduleId) => null;

        public IDynamicCode OfModule(int pageId, int moduleId) => null;
    }
}
