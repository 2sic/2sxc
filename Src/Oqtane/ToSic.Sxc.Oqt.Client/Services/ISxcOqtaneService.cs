using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public interface ISxcOqtaneService
    {
        Task<SxcOqtaneDto> PrepareAsync(int aliasId, int siteId, int pageId, int moduleId);

        SxcOqtaneDto Prepare(int aliasId, int siteId, int pageId, int moduleId);
    }
}
