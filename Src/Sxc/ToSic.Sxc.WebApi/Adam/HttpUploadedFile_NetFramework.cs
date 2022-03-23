#if NETFRAMEWORK

using System;
using System.IO;
using System.Net.Http;
using System.Web;
using Microsoft.EntityFrameworkCore.Internal;
using ToSic.Eav.Security.Files;

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

        public int Count => Request.Files.Count;

        public (string, Stream) GetStream(int i = 0)
        {
            var file = Request.Files[i];

            var fileName = FileNames.SanitizeFileName(file?.FileName);

            if (FileNames.IsKnownRiskyExtension(fileName))
                throw new Exception($"File {fileName} has risky file type.");

            return (fileName, file?.InputStream);
        }
    }
}

#endif