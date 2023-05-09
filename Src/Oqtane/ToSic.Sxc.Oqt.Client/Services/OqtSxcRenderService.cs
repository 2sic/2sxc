using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System.Net.Http;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class OqtSxcRenderService : ServiceBase, IService
    {
        public OqtSxcRenderService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string ApiUrl => CreateApiUrl("OqtSxcRender");

        public async Task<OqtViewResultsDto> PrepareAsync(int aliasId, int pageId, int moduleId, string culture,
            string query, bool preRender)
        {
            var url = CreateAuthorizationPolicyUrl($"{ApiUrl}/{aliasId}/{pageId}/{moduleId}/{culture}/{preRender}/Prepare{query}", EntityNames.Module, moduleId);
            return await GetJsonAsync<OqtViewResultsDto>(url);
        }
    }
}
