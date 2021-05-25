using System;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class PageLookUp : LookUpBase
    {
        private readonly Lazy<OqtState> _oqtState;
        protected Oqtane.Models.Page Page { get; set; }

        public PageLookUp(Lazy<OqtState> oqtState)
        {
            Name = "Page";

            _oqtState = oqtState;
        }

        public Oqtane.Models.Page GetSource()
        {
            var ctx = _oqtState.Value.GetContext();
            return ((OqtPage) ctx.Page).UnwrappedContents;
        }

        public override string Get(string key, string format)
        {
            try
            {
                Page ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Page.PageId}",
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