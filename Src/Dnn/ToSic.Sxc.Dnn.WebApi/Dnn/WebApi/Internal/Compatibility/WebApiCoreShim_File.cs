using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Coding;
using ToSic.Sxc.Backend;

namespace ToSic.Sxc.Dnn.WebApi.Internal.Compatibility;

partial class WebApiCoreShim
{
    /// <inheritdoc />
    public dynamic File(NoParamOrder noParamOrder = default,
        // Important: the second parameter should _not_ be a string, otherwise the signature looks the same as the built-in File(...) method
        bool? download = null,
        string virtualPath = null, // important: this is the virtualPath, but it should not have the same name, to not confuse the compiler with same sounding param names
        string contentType = null,
        string fileDownloadName = null,
        object contents = null // can be stream, string or byte[]
    )
    {
        // fileDownloadName becomes null when download != true

        // Try to figure out file mime type as needed
        if (string.IsNullOrWhiteSpace(contentType))
            contentType = (string.IsNullOrWhiteSpace(fileDownloadName ?? virtualPath)) ? MimeHelper.FallbackType : MimeMapping.GetMimeMapping(fileDownloadName ?? virtualPath);

        HttpContent httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(string.Empty));

        // check if this may just be a call to the built in file, which has two strings
        // this can only be possible if only the virtualPath and contentType were set
        if (!string.IsNullOrWhiteSpace(virtualPath))
            httpContent = new StreamContent(new FileStream(HttpContext.Current.Server.MapPath(virtualPath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

        var encoding = Encoding.UTF8; // the default response encoding is UTF-8
        var isValidXml = false;
        switch (contents)
        {
            case XmlDocument xmlDoc:
                var xmlStream = new MemoryStream();
                xmlDoc.Save(xmlStream);
                xmlStream.Position = 0;
                httpContent = new StreamContent(xmlStream);
                isValidXml = true;
                encoding = CustomApiHelpers.GetEncoding(xmlDoc);
                break;
            case string stringBody:
                httpContent = new ByteArrayContent(encoding.GetBytes(stringBody));
                isValidXml = CustomApiHelpers.IsValidXml(stringBody);
                encoding = CustomApiHelpers.GetEncoding(stringBody);
                break;
            case Stream streamBody:
                httpContent = new StreamContent(streamBody);
                isValidXml = CustomApiHelpers.IsValidXml(streamBody);
                encoding = CustomApiHelpers.GetEncoding(streamBody);
                break;
            case byte[] charBody:
                httpContent = new ByteArrayContent(charBody);
                isValidXml = CustomApiHelpers.IsValidXml(charBody);
                encoding = CustomApiHelpers.GetEncoding(charBody);
                break;
        }
        contentType = CustomApiHelpers.XmlContentTypeFromContent(isValidXml, contentType);

        var response = Request.CreateResponse(HttpStatusCode.OK);
        response.Content = httpContent;
        response.Content.Headers.ContentType = CustomApiHelpers.PrepareMediaTypeHeaderValue(contentType, encoding);
        response.Content.Headers.ContentDisposition = CustomApiHelpers.PrepareContentDispositionHeaderValue(download, fileDownloadName);

        return response;
    }
}