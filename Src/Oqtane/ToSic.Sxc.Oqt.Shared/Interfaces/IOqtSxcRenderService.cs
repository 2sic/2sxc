using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IOqtSxcRenderService
{
    Task<OqtViewResultsDto> RenderAsync(RenderParameters @params);

    OqtViewResultsDto Render(RenderParameters @params);
}