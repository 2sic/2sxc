using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oqtane.Models;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public static class OqtContextOfBlockExtensions
    {
        public static IContextOfBlock Init(this IContextOfBlock context, int pageId, Module oqtModule, ILog parentLog)
        {
            context.Init(parentLog);
            ((OqtPage)context.Page).Init(pageId);
            ((OqtModule)context.Module).Init(oqtModule, parentLog);
            return context; // InitPageOnly(context);
        }

        //public static IContextOfBlock InitPageOnly(this IContextOfBlock context)
        //{
        //    // Collect / assemble page information
        //    //var activeTab = (context.Site as Site<PortalSettings>)?.UnwrappedContents?.ActiveTab;
        //    context.Page.Init(activeTab?.TabID ?? Eav.Constants.NullId);

        //    // the FullUrl will throw an error in DNN search scenarios
        //    try
        //    {
        //        //((Page)context.Page).Url = activeTab?.FullUrl;
        //    }
        //    catch
        //    {
        //        /* ignore */
        //    }

        //    return context;
        //}

    }
}
