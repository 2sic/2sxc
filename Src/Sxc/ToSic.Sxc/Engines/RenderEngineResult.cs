using System;
using System.Collections.Generic;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    public class RenderEngineResult
    {
        public RenderEngineResult(string html, bool activateJsApi, List<IClientAsset> assets, string errorCode, Exception exOrNull)
        {
            Html = html;
            ActivateJsApi = activateJsApi;
            Assets = assets ?? new List<IClientAsset>();
            ErrorCode = errorCode;
            ExceptionOrNull = exOrNull;
        }

        public string Html { get; }

        public bool ActivateJsApi { get; }

        public List<IClientAsset> Assets { get; }

        public string ErrorCode { get; }
        public Exception ExceptionOrNull { get; }
    }
}
