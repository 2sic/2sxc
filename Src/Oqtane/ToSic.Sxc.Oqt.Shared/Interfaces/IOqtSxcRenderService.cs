using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOqtSxcRenderService
{
    Task<OqtViewResultsDto> PrepareAsync(int aliasId, int pageId, int moduleId, string culture, bool preRender, string originalParameters);

    OqtViewResultsDto Prepare(int aliasId, int pageId, int moduleId, string culture, bool preRender, string originalParameters);
}