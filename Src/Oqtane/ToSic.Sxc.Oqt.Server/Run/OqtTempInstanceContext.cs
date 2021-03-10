using System;
using Oqtane.Models;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtTempInstanceContext
    {
        private readonly IServiceProvider _serviceProvider;

        public OqtTempInstanceContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IContextOfBlock CreateContext(int pageId, Module module, ILog parentLog)
        {
            var ctx = _serviceProvider.Build<IContextOfBlock>();
            ctx.Init(parentLog);
            ctx.Page.Init(pageId);
            ((OqtModule) ctx.Module).Init(module, parentLog);
            return ctx;
        }
    }
}
