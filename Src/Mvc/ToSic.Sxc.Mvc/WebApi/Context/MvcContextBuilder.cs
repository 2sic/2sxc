using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Run;

using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.Mvc.WebApi.Context
{
    public class MvcContextBuilder: UiContextBuilderBase
    {
        public MvcContextBuilder(ILinkPaths linkPaths, Dependencies deps) : base(deps)
        {
            _linkPaths = linkPaths;
        }

        internal MvcContextBuilder Init(IBlock block)
        {
            _context = block.Context;
            SetZoneAndApp(block.ZoneId, block.App);
            return this;
        }

        private readonly ILinkPaths _linkPaths;
        private IContextOfBlock _context;


        protected override LanguageDto GetLanguage()
        {
            return new LanguageDto
            {
                Current = TestIds.DefaultLanguage,
                Primary = TestIds.DefaultLanguage,
                All = new Dictionary<string, string>
                {
                    {TestIds.DefaultLanguage, TestIds.DefaultLanguageText}
                }
            };
        }

        protected override WebResourceDto GetSystem() =>
            new WebResourceDto
            {
                Url = _linkPaths.ToAbsolute("~/")
            };

        protected override WebResourceDto GetSite() =>
            new WebResourceDto
            {
                Id = _context.Site.Id,
                Url = "//" + _context.Site.Url,
            };

        protected override WebResourceDto GetPage() =>
            new WebResourceDto
            {
                Id = _context.Page.Id,
            };

        protected override EnableDto GetEnable()
        {
            return new EnableDto
            {
                AppPermissions = true,
                CodeEditor = true,
                Query = true
            };
        }

        protected override string GetGettingStartedUrl() => "#todo-not-yet-implemented-getting-started";
    }
}
