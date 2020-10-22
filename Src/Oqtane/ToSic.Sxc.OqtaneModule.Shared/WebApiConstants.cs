using System;
using System.Collections.Generic;
using System.Text;

namespace ToSic.Sxc.OqtaneModule
{
    public class WebApiConstants /*: ToSic.Sxc.WebApi.WebApiConstants*/
    {
        public const string WebApiRoot = "api/sxc";
        public const string WebApiStateRoot = "{alias}/api/sxc";
        public const string WebApiDefaultRoute = WebApiRoot + "/[controller]/[action]";

        public const string MvcApiLogPrefix = "MAP.";
    }
}
