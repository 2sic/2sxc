using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Run
{
    public interface ISxcOqtane
    {
        void Prepare(Site site, Page page, Module module);

        MarkupString GeneratedHtml { get; }

        IOqtAssetsAndHeader AssetsAndHeaders { get; }
        
        List<SxcResource> Resources { get; }

        string Test();
    }
}
