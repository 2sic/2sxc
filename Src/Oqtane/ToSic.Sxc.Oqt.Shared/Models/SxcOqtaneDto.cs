using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using ToSic.Sxc.Oqt.Shared.Run;

namespace ToSic.Sxc.Oqt.Shared.Models
{
    public class SxcOqtaneDto
    {
        public MarkupString GeneratedHtml { get; set;  }

        public IOqtAssetsAndHeader AssetsAndHeaders { get; set; }

        public List<SxcResource> Resources { get; set; }
    }
}
