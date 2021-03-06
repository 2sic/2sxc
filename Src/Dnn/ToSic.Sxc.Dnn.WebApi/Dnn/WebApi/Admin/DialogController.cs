﻿using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class DialogController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.SysCnt";

        #region Dialog Helpers
        /// <summary>
        /// This is the subsystem which delivers the getting-started app-iframe with instructions etc.
        /// Used to be GET System/DialogSettings
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public DialogContextStandalone Settings(int appId) 
            => GetService<AdminBackend>().Init(Log).DialogSettings(appId);

        #endregion

    }
}