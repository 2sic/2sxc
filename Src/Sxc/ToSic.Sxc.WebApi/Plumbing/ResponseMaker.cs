using System;
#if NETFRAMEWORK
using PlatformResponseType = System.Net.Http.HttpResponseMessage;
#else
using PlatformResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.WebApi.Plumbing
{
    public abstract class ResponseMaker
    {
        public abstract PlatformResponseType InternalServerError(string message);
        public abstract PlatformResponseType InternalServerError(Exception exception);
        
        public abstract PlatformResponseType Error(int statusCode, string message);
        
        public abstract PlatformResponseType Error(int statusCode, Exception exception);
        public abstract PlatformResponseType Json(object json);
    }
}
