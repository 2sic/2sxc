using System.Net;
#if NETFRAMEWORK
using System.Net.Http;
using System.Web.Http;
using BaseType = System.Web.Http.HttpResponseException;
#else
using BaseType = System.Exception;
#endif
namespace ToSic.Sxc.WebApi.Errors
{
    internal class HttpExceptionAbstraction: BaseType
    {
        //public HttpStatusCode StatusCode;
        //public string Message;

        public HttpExceptionAbstraction(HttpStatusCode statusCode, string message)
#if NETFRAMEWORK
            : base(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(message),
                ReasonPhrase = "Error in 2sxc Content API - not allowed"
            })
#else
            : base("Error " + statusCode.ToString() + " " + message)
#endif

        {
            //StatusCode = statusCode;
            //Message = message;
        }

//        public Exception ToException()
//        {
//#if NETFRAMEWORK
//            var err = new HttpResponseException(new HttpResponseMessage(StatusCode)
//            {
//                Content = new StringContent(Message),
//                ReasonPhrase = "Error in 2sxc Content API - not allowed"
//            });
//            return err;
//#else
//            return new Exception(Message);
//#endif
//        }
    }
}
