using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System.Net.Http;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OqtSxcRenderService(HttpClient http, SiteState siteState) : ServiceBase(http, siteState), IOqtSxcRenderService, IClientService
{
    private string ApiUrl => CreateApiUrl("OqtSxcRender");

    public async Task<OqtViewResultsDto> RenderAsync(RenderParameters @params)
    {
        var url = CreateAuthorizationPolicyUrl($"{ApiUrl}/{@params.AliasId}/{@params.PageId}/{@params.ModuleId}/{@params.Culture}/{@params.PreRender}/Render{@params.OriginalParameters}", EntityNames.Module, @params.ModuleId);
        return await GetJsonAsync<OqtViewResultsDto>(url);
    }

    public OqtViewResultsDto Render(RenderParameters @params) => RenderAsync(@params).GetAwaiter().GetResult();
}