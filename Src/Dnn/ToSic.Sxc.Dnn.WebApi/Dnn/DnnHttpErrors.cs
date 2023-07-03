using System;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using ToSic.Sxc.Code.Help;

namespace ToSic.Sxc.Dnn
{
    internal class DnnHttpErrors
    {
        internal static HttpResponseException LogAndReturnException(
            HttpRequestMessage request,
            HttpStatusCode code,
            Exception e,
            string msg,
            CodeErrorHelpService errorHelp)
        {
            var helpText = errorHelp.FindHelp(e)?.ErrorMessage + msg;
            var exception = new Exception(helpText, e);
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);

            var dnnUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            if (dnnUser?.IsSuperUser == true)
            {

            }
            return new HttpResponseException(request.CreateErrorResponse(code, helpText, e));
        }

    }
}
