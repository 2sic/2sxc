using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.UI.Modules;
using DotNetNuke.Services.Localization;

namespace ToSic.SexyContent.Razor.Helpers
{
    public class HtmlHelper
    {
        public HtmlString Raw(string text)
        {
            return new HtmlString(text);
        }

        /// <summary>
        /// Temporary overload needed for backward compatibility Html.Raw(Content.Toolbar)
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public HtmlString Raw(HtmlString htmlString)
        {
            return htmlString;
        }
    }
}