using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace ToSic.SexyContent.WebApi
{
    public class SxcWebApiExceptionHandling : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = new Exception("An error occurred while executing the request. Please consult the event log for details.", context.Exception);
            // Log errors to DNN log
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);

            throw exception;
        }
    }
}