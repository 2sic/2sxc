using DotNetNuke.Common;
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
            => Globals.NavigateURL(_context.PageId);

        public string NavigateToControl(string controlKey) 
            => Globals.NavigateURL(_context.PageId, controlKey, $"mid={_context.Id}");
    }
}