using System.Net;
using ToSic.Lib.Exceptions;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;

namespace ToSic.Sxc.Dnn.Integration;

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
        return new(request.CreateErrorResponse(code, exToShow));
    }

}