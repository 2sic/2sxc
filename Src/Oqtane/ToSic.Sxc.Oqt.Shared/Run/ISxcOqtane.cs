using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Oqtane.Models;

namespace ToSic.Sxc.Oqt.Shared.Run
{
    public interface ISxcOqtane
    {
        void Prepare(Site site, Page page, Module module);

        MarkupString GeneratedHtml { get; }

        IOqtAssetsAndHeader AssetsAndHeaders { get; }
        
        List<Resource> Resources { get; }

        string Test();
    }
}
