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
        private readonly string _resourceFile;
        private ModuleInstanceContext _context;

        public HtmlHelper(ModuleInstanceContext context, string resourcefile)
        {
            _context = context;
            _resourceFile = resourcefile;
        }

        public object GetLocalizedString(string key)
        {
            return Localization.GetString(key, _resourceFile);
        }

        public object GetLocalizedString(string key, string culture)
        {
            return Localization.GetString(key, _resourceFile, culture);
        }

        public HtmlString Raw(string text)
        {
            return new HtmlString(text);
        }
    }
}