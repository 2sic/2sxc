using System;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using ToSic.Lib.Exceptions;
using ToSic.Sxc.Code.Help;

namespace ToSic.Sxc.Dnn
{
    internal class DnnHttpErrors
    {
        private const string ApiErrPrefix = "2sxc Api Controller Finder Error: ";
        private const string ApiErrGeneral = "Error selecting / compiling an API controller. ";
        private const string ApiErrSuffix = "Check event-log, code and inner exception. ";

        internal const string ApiErrMessage = ApiErrPrefix + ApiErrGeneral + ApiErrSuffix;

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
            var exToShow = dnnUser?.IsSuperUser == true 
                ? exception 
                : new PublicException(ApiErrMessage + helpText);
            return new HttpResponseException(request.CreateErrorResponse(code, exToShow));
        }

    }
}
