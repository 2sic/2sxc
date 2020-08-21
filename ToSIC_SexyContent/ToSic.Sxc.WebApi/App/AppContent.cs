using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.WebApi.App
{
    public class AppContent: HasLog
    {
        public AppContent(ILog parentLog = null) : base("Sxc.ApiApC", parentLog)
        {
        }

    }
}
