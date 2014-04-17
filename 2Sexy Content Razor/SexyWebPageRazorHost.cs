using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToSic.SexyContent.Razor
{
    class SexyWebPageRazorHost : System.Web.WebPages.Razor.WebPageRazorHost
    {
        public SexyWebPageRazorHost(string virtualPath)
            : base(virtualPath, null)
        {
            this.RegisterSpecialFile();
        }
    }
}
