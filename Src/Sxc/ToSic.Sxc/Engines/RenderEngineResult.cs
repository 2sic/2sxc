using System;
using System.Collections.Generic;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    public class RenderEngineResult
    {
        public RenderEngineResult(RenderEngineResult original, string html = default, bool activateJsApi = default, List<IClientAsset> assets = default, string errorCode = default, Exception exOrNull = default)
        : this(original?.Html ?? html, original?.ActivateJsApi ?? activateJsApi, original?.Assets ?? assets, original?.ErrorCode ?? errorCode, original?.ExceptionOrNull ?? exOrNull)
        {
        }

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
