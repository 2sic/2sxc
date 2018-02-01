using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;

namespace ToSic.SexyContent.Razor.Helpers
{
    public class UrlHelper
    {
        private readonly IInstanceInfo _context;

        public UrlHelper(IInstanceInfo context)
        {
            _context = context;
        }

        public string NavigateToControl()
        {
            return Globals.NavigateURL(_context.PageId/*.TabID*/);
        }

        public string NavigateToControl(string controlKey)
        {
            return Globals.NavigateURL(_context.PageId/*.TabID*/, controlKey, "mid=" + _context.Id/*.ModuleID*/);
        }
    }
}