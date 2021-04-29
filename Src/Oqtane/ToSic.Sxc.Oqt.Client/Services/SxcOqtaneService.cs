using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Oqt.Shared.Run;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class SxcOqtaneService : ServiceBase, ISxcOqtaneService, IService
    {
        private readonly SiteState _siteState;

        public SxcOqtaneService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        private string Apiurl => CreateApiUrl(_siteState.Alias, "SxcOqtane");

        public async Task<SxcOqtaneDto> PrepareAsync(int aliasId, int siteId, int pageId, int moduleId)
        {
            return await GetJsonAsync<SxcOqtaneDto>($"{Apiurl}/Prepare?entityid={moduleId}&aliasId={aliasId}&siteId={siteId}&pageId={pageId}&moduleId={moduleId}");
        }

        public SxcOqtaneDto Prepare(int aliasId, int siteId, int pageId, int moduleId)
        {
            var rez = GetJsonAsync<SxcOqtaneDto>($"{Apiurl}/Prepare?entityid={moduleId}&aliasId={aliasId}&siteId={siteId}&pageId={pageId}&moduleId={moduleId}")
                .AwaitResult();
            return rez;
            // sync call not working, hangs
            //return GetJsonAsync<SxcOqtaneDto>($"{Apiurl}/Prepare?a={aliasId}&s={siteId}&p={pageId}&m={moduleId}")
            //    .Result;

            // sync call
            //var task = Task.Run(async () => await GetJsonAsync<SxcOqtaneDto>($"{Apiurl}/Prepare?a={aliasId}&s={siteId}&p={pageId}&m={moduleId}"));
            //task.Wait();
            //return task.Result;
        }
    }
}
