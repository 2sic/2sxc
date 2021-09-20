using ToSic.Eav.LookUp;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    // TODO: @STV pls check - not using oqtState any more
    // Afterwards pls Rename to OqtPageLookUp
    public class PageLookUp : LookUpBase
    {
        private readonly IContextResolver _ctxResolver;
        protected Oqtane.Models.Page Page { get; set; }

        public PageLookUp(IContextResolver ctxResolver)
        {
            Name = "Page";
            _ctxResolver = ctxResolver;
        }

        public Oqtane.Models.Page GetSource()
        {
            if (_alreadyTried) return null;
            _alreadyTried = true;
            var ctx = _ctxResolver.BlockOrNull();
            return ((OqtPage) ctx.Page).UnwrappedContents;
        }
        private bool _alreadyTried;

        public override string Get(string key, string format)
        {
            try
            {
                Page ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Page.PageId}",     // Todo: must ensure id will also work in Dnn implementation
                    "pageid" => "Warning: 'PageId' was requested, but the page source can only answer to Id", // warning for compatibility with old Dnn implementation
                    "url" => $"{Page.Url}",
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}