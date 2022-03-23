#if NETSTANDARD
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using ToSic.Eav.Security.Files;

namespace ToSic.Sxc.WebApi.Adam
{
    public class HttpUploadedFile
    {
        public HttpUploadedFile(HttpRequest request) => Request = request;
        public HttpRequest Request { get; }

        // https://stackoverflow.com/questions/45871479/net-core-2-how-to-check-if-the-request-is-mime-multipart-content
        public bool IsMultipart() => Request.GetMultipartBoundary() != null;

        public bool HasFiles() => Request.Form.Files.Any();

        public int Count => Request.Form.Files.Count;

        public (string, Stream) GetStream(int i = 0)
        {
            var file = Request.Form.Files[i];

            var fileName = FileNames.SanitizeFileName(file?.FileName);

            if (FileNames.IsKnownRiskyExtension(fileName))
                throw new Exception($"File {fileName} has risky file type.");

            return (fileName, file.OpenReadStream());
        }

    }
}
#endif