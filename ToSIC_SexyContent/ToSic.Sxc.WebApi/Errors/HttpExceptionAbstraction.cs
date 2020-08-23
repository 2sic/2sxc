using System.Net;
#if NETFRAMEWORK
using System.Net.Http;
using BaseType = System.Web.Http.HttpResponseException;
#else
using BaseType = System.Exception;
#endif
namespace ToSic.Sxc.WebApi.Errors
{
    internal class HttpExceptionAbstraction: BaseType
    {
        public HttpExceptionAbstraction(HttpStatusCode statusCode, string message, string title = null)
#if NETFRAMEWORK
            : base(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(message),
                ReasonPhrase = title ?? "Error in 2sxc Content API - not allowed"
            })
#else
            : base("Error " + statusCode.ToString() + " " + message)
#endif

        {
        }
    }
}
