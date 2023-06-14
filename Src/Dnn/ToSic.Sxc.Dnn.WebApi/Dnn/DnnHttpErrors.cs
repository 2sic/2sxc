using System;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using ToSic.Sxc.Code.Errors;

namespace ToSic.Sxc.Dnn
{
    internal class DnnHttpErrors
    {
        internal static HttpResponseException LogAndReturnException(HttpRequestMessage request, HttpStatusCode code, Exception e, string msg, CodeErrorHelpService errorHelp)
        {
            var helpText = errorHelp.HelpText(e) + msg;
            var exception = new Exception(helpText, e);
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
            return new HttpResponseException(request.CreateErrorResponse(code, helpText, e));
        }

    }
}
