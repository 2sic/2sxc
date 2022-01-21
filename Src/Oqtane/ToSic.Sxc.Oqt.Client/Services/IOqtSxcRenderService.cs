using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public interface IOqtSxcRenderService
    {
        Task<OqtViewResultsDto> PrepareAsync(int aliasId, int pageId, int moduleId, string culture, string originalParameters);
    }
}
