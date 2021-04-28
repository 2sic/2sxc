using Microsoft.AspNetCore.Components;
using Oqtane.Models;
using System.Collections.Generic;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Run
{
    public interface ISxcOqtane
    {
        void Prepare(Alias alias, Site site, Page page, Module module);

        MarkupString GeneratedHtml { get; }

        IOqtAssetsAndHeader AssetsAndHeaders { get; }

        List<SxcResource> Resources { get; }

        string Test();
    }
}
