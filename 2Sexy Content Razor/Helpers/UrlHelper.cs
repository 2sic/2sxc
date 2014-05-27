using DotNetNuke.Entities.Modules;
using DotNetNuke.Common;

namespace ToSic.SexyContent.Razor.Helpers
{
    public class UrlHelper
    {
        private readonly ModuleInfo _context;

        public UrlHelper(ModuleInfo context)
        {
            _context = context;
        }

        public string NavigateToControl()
        {
            return Globals.NavigateURL(_context.TabID);
        }

        public string NavigateToControl(string controlKey)
        {
            return Globals.NavigateURL(_context.TabID, controlKey, "mid=" + _context.ModuleID);
        }
    }
}