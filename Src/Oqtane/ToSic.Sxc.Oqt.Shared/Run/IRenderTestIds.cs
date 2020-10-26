using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Shared.Run
{
    public interface IRenderTestIds
    {
        void Prepare(Site site, Page page, Module module);

        MarkupString GeneratedHtml { get; }

        //MarkupString RenderModule(Site site, Page page, Module module);

        IOqtAssetsAndHeader AssetsAndHeaders { get; }

        string Test();
    }
}
