#if NETFRAMEWORK

using System.IO;
using System.Net.Http;
using System.Web;
using Microsoft.EntityFrameworkCore.Internal;

namespace ToSic.Sxc.WebApi.Adam
{
    public class HttpUploadedFile
    {
        public HttpUploadedFile(HttpRequestMessage requestMessage, HttpRequest request)
        {
            RequestMessage = requestMessage;
            Request = request;
        }

        public HttpRequestMessage RequestMessage { get; }
        public HttpRequest Request { get; }

        public bool IsMultipart() => RequestMessage.Content.IsMimeMultipartContent();

        public bool HasFiles() => Request.Files.Any();

        public (string, Stream) GetStream()
        {
            var originalFile = Request.Files[0];
            return (originalFile?.FileName, originalFile?.InputStream);
        }
    }
}

#endif