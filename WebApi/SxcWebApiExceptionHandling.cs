using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace ToSic.SexyContent.WebApi
{
    public class SxcWebApiExceptionHandling : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception;
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);

            // special manual exception maker, because otherwise IIS at runtime removes all messages
            // without this, debug-infos like what entity is causing the problem will not be shown to the client
            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                                                               "Bad Request",
                                                               context.Exception);
            var httpError = (HttpError)((ObjectContent<HttpError>)context.Response.Content).Value;
            if (!httpError.ContainsKey("ExceptionType"))
                httpError.Add("ExceptionType", exception.GetType().FullName);
            if (!httpError.ContainsKey("ExceptionMessage"))
                httpError.Add("ExceptionMessage", exception.Message);
        }
    }
}