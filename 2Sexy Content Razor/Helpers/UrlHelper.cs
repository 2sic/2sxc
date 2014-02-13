using DotNetNuke.UI.Modules;
using DotNetNuke.Common;

namespace ToSic.SexyContent.Razor.Helpers
{
    public class UrlHelper
    {
        private readonly ModuleInstanceContext _context;

        public UrlHelper(ModuleInstanceContext context)
        {
            _context = context;
        }

        public string NavigateToControl()
        {
            return Globals.NavigateURL(_context.TabId);
        }

        public string NavigateToControl(string controlKey)
        {
            return Globals.NavigateURL(_context.TabId, controlKey, "mid=" + _context.ModuleId);
        }
    }
}