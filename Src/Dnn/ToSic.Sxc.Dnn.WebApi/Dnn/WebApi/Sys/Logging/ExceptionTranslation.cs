using System.Net;
using System.Web.Http.Filters;
using ToSic.Sxc.Adam.Sys.Security;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

/// <summary>
/// Helper to translate common internal errors to proper HTTP Exceptions
/// </summary>
internal static class ExceptionTranslation
{
    public static HttpResponseMessage ExceptionToResponse(HttpActionExecutedContext context)
        => context.Exception switch
        {
            ArgumentException or FormatException => context.Request.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                context.Exception.Message),
            KeyNotFoundException => context.Request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                context.Exception.Message),
            NotAllowedFileTypeException => context.Request.CreateErrorResponse(
                HttpStatusCode.UnsupportedMediaType,
                context.Exception.Message),
            PermissionDeniedException pdEx => context.Request.CreateErrorResponse(
                HttpStatusCode.Forbidden,
                pdEx.Message.NullIfNoValue() ?? "You do not have permission to perform this action.",
                context.Exception),
            UnauthorizedAccessException => context.Request.CreateErrorResponse(
                HttpStatusCode.Forbidden,
                "You do not have permission to perform this action.",
                context.Exception),
            // special manual exception maker, because otherwise IIS at runtime removes all messages
            // without this, debug-infos like what entity is causing the problem will not be shown to the client
            _ => context.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                "Bad Request",
                context.Exception),
            //_ => context.Response
        };
}
