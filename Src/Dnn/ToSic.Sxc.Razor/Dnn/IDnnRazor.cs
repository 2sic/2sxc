using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.Sxc.Dnn
{
    public interface IDnnRazor
    {
        /// <summary>
        /// Helper for Html.Raw - for creating raw html output which doesn't encode &gt; and &lt;
        /// </summary>
        IHtmlHelper Html { get; }

    }
}
