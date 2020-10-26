using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Shared.Run
{
    public interface IRenderTestIds
    {
        MarkupString RenderHtml(InstanceId id);

        MarkupString RenderModule(Site site, Page page, Module module);

        IOqtAssetsAndHeader AssetsAndHeaders { get; }

        string Test();
    }
}
