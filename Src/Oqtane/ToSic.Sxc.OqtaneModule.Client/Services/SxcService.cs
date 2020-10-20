using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using ToSic.Sxc.OqtaneModule.Models;

namespace ToSic.Sxc.OqtaneModule.Services
{
    public class SxcService : ServiceBase, ISxcService, IService
    {
        private readonly SiteState _siteState;

        public SxcService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

         private string Apiurl => CreateApiUrl(_siteState.Alias, "Sxc");

        public async Task<List<Models.Sxc>> GetSxcsAsync(int ModuleId)
        {
            List<Models.Sxc> Sxcs = await GetJsonAsync<List<Models.Sxc>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", ModuleId));
            return Sxcs.OrderBy(item => item.Name).ToList();
        }

        public async Task<Models.Sxc> GetSxcAsync(int SxcId, int ModuleId)
        {
            return await GetJsonAsync<Models.Sxc>(CreateAuthorizationPolicyUrl($"{Apiurl}/{SxcId}", ModuleId));
        }

        public async Task<Models.Sxc> AddSxcAsync(Models.Sxc Sxc)
        {
            return await PostJsonAsync<Models.Sxc>(CreateAuthorizationPolicyUrl($"{Apiurl}", Sxc.ModuleId), Sxc);
        }

        public async Task<Models.Sxc> UpdateSxcAsync(Models.Sxc Sxc)
        {
            return await PutJsonAsync<Models.Sxc>(CreateAuthorizationPolicyUrl($"{Apiurl}/{Sxc.SxcId}", Sxc.ModuleId), Sxc);
        }

        public async Task DeleteSxcAsync(int SxcId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{SxcId}", ModuleId));
        }
    }
}
