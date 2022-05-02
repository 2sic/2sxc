using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Very experimental, do not use
    /// </summary>
    [PrivateApi]
    public class CspService : CspServiceBase, ICspService
    {
        public CspService(PageServiceShared pageServiceShared)
        {
            _pageSvcShared = pageServiceShared;
            pageServiceShared.Csp.AddCspService(this);
        }
        private readonly PageServiceShared _pageSvcShared;

        public override bool IsEnforced => _pageSvcShared.Csp.IsEnforced;

        public override bool IsEnabled => _pageSvcShared.Csp.IsEnabled;
        
        
    }
}
