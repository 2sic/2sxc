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

    public async Task<OqtViewResultsDto> PrepareAsync(int aliasId, int pageId, int moduleId, string culture, bool preRender, string originalParameters)
    {
        var url = CreateAuthorizationPolicyUrl($"{ApiUrl}/{aliasId}/{pageId}/{moduleId}/{culture}/{preRender}/Prepare{originalParameters}", EntityNames.Module, moduleId);
        return await GetJsonAsync<OqtViewResultsDto>(url);
    }

    public OqtViewResultsDto Prepare(int aliasId, int pageId, int moduleId, string culture, bool preRender, string originalParameters)
        => PrepareAsync(aliasId, pageId, moduleId, culture, preRender, originalParameters).GetAwaiter().GetResult();
}