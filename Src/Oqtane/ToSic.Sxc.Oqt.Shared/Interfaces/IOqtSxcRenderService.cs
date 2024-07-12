using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IOqtSxcRenderService
{
    Task<OqtViewResultsDto> RenderAsync(RenderParameters @params);

    OqtViewResultsDto Render(RenderParameters @params);
}