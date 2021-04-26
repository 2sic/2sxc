using Microsoft.AspNetCore.Http;
using System;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtAppFolder: HasLog<OqtAppFolder>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;


        public OqtAppFolder(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) : base($"{OqtConstants.OqtLogPrefix}.AppFolder")
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public string GetAppFolder()
        {
            HttpRequest GetRequest() =>  _httpContextAccessor.HttpContext.Request;
            var oqtState = new OqtState(GetRequest, _serviceProvider, Log);
            var ctx = oqtState.GetContext();
            return ctx.AppState.Folder;
        }
    }
}
