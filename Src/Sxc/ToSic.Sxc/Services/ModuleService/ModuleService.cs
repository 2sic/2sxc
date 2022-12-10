using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services
{
    [PrivateApi]
    public class ModuleService: HasLog, IModuleService
    {
        public ModuleService() : base(Constants.SxcLogName + ".ModSvc") { }

        public void AddToMore(IHtmlTag tag) => _moreTags.Add(tag);

        public IReadOnlyCollection<IHtmlTag> MoreTags => _moreTags;
        private readonly List<IHtmlTag> _moreTags = new List<IHtmlTag>();
    }
}
