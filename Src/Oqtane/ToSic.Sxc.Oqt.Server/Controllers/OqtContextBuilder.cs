using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Run;

using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public class OqtContextBuilder: ContextBuilderBase
    {
        public OqtContextBuilder(ILinkPaths linkPaths)
        {
            _linkPaths = linkPaths;
        }

        internal OqtContextBuilder Init(IContextOfSite ctx, IApp app = null)
        {
            _context = ctx;
            InitApp(ctx.Site.ZoneId, app);
            return this;
        }

        private readonly ILinkPaths _linkPaths;
        private IContextOfSite _context;


        protected override LanguageDto GetLanguage()
        {
            return new LanguageDto
            {
                Current = WipConstants.DefaultLanguage,
                Primary = WipConstants.DefaultLanguage,
                //All = new Dictionary<string, string>
                //{
                //    {WipConstants.DefaultLanguage, WipConstants.DefaultLanguageText}
                //}
                All = WipConstants.EmptyLanguages,
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
                Id = (_context as IContextOfBlock)?.Page.Id ?? Eav.Constants.NullId,
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
