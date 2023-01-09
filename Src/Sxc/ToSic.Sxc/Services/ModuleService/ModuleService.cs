using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services
{
    [PrivateApi]
    public class ModuleService: ServiceBase, IModuleService
    {
        public ModuleService() : base(Constants.SxcLogName + ".ModSvc") { }

        public void AddToMore(IHtmlTag tag) => _moreTags.Add(tag);

        public IReadOnlyCollection<IHtmlTag> MoreTags => _moreTags;
        private readonly List<IHtmlTag> _moreTags = new List<IHtmlTag>();
    }
}
