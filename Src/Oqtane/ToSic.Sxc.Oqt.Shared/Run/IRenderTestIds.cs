using Microsoft.AspNetCore.Components;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Shared.Run
{
    public interface IRenderTestIds
    {
        MarkupString RenderHtml(InstanceId id);

    }
}
