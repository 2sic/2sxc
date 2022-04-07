using System.Collections.Generic;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    public class RenderEngineResult
    {
        public RenderEngineResult(string html, bool activateJsApi, List<IClientAsset> assets)
        {
            Html = html;
            ActivateJsApi = activateJsApi;
            Assets = assets ?? new List<IClientAsset>();
        }

        public string Html { get; set; }

        public bool ActivateJsApi { get; set; }

        public List<IClientAsset> Assets { get; set; }
    }
}
