using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System.Net.Http;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class OqtSxcRenderService : ServiceBase, IOqtSxcRenderService, IService
    {
        private readonly SiteState _siteState;

        public OqtSxcRenderService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        private string Apiurl => CreateApiUrl(_siteState.Alias, "OqtSxcRender");

        public async Task<OqtViewResultsDto> PrepareAsync(int aliasId, int pageId, int moduleId, string culture,
            string query)
        {
            return await GetJsonAsync<OqtViewResultsDto>(CreateAuthorizationPolicyUrl($"{Apiurl}/{aliasId}/{pageId}/{moduleId}/{culture}/Prepare{query}", moduleId));
        }
    }
}
