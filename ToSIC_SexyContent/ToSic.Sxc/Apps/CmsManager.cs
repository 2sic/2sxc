using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager
    {
        public CmsManager(IAppIdentity app, ILog parentLog) : base(app, parentLog)
        {
        }

        /// <summary>
        /// The template management subsystem
        /// </summary>
        [PrivateApi("in our official Architecture there should be no templates at the Eav.Apps level.")]
        public TemplatesManager Templates => _templates ?? (_templates = new TemplatesManager(this, Log));
        private TemplatesManager _templates;


    }
}
