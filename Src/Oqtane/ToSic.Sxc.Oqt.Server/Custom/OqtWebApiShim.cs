using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToSic.Lib.Coding;
using ToSic.Sxc.Backend;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Server.Custom;

internal class OqtWebApiShim(HttpResponse response, ControllerBase owner)
{
    public HttpResponse Response { get; } = response;

    public dynamic File(NoParamOrder noParamOrder = default,
        // Important: the second parameter should _not_ be a string, otherwise the signature looks the same as the built-in File(...) method
        bool? download = null,
        string virtualPath = null, // important: this is the virtualPath, but it should not have the same name, to not confuse the compiler with same sounding param names
        string contentType = null,
        string fileDownloadName = null,
        object contents = null // can be stream, string or byte[]
    )
    {
        fileDownloadName = CustomApiHelpers.FileParamsInitialCheck(noParamOrder, download, virtualPath, fileDownloadName, contents);

        // Try to figure out file mime type as needed
        if (string.IsNullOrWhiteSpace(contentType))
            contentType = OqtAssetsFileHelper.GetMimeType(fileDownloadName ?? virtualPath);

        // check if this may just be a call to the built in file, which has two strings
        // this can only be possible if only the virtualPath and contentType were set
        if (!string.IsNullOrWhiteSpace(virtualPath))
            return owner.File(virtualPath, contentType, fileDownloadName);

        // add only header "Content-Disposition: inline, file..."
        if (download != true)
            Response.Headers.Add("Content-Disposition", CustomApiHelpers.PrepareContentDispositionHeaderValue(download, fileDownloadName).ToString());

        // in aspNetCore for File stream/content result in response
        // fileDownloadName should be null to get header "Content-Disposition: inline"
        // that will directly show content response in browser
        // opposed to "Content-Disposition: attachment; filename=..." that file start file download
        fileDownloadName = CustomApiHelpers.EnsureFileDownloadNameIsNullForInline(download, fileDownloadName);
        Encoding encoding;
        string mediaTypeHeaderValue;

        switch (contents)
        {
            case XmlDocument xmlDoc:
                var xmlStream = new MemoryStream();
                xmlDoc.Save(xmlStream);
                xmlStream.Position = 0;
                contentType = CustomApiHelpers.XmlContentTypeFromContent(true, contentType);
                encoding = CustomApiHelpers.GetEncoding(xmlDoc);
                mediaTypeHeaderValue = CustomApiHelpers.PrepareMediaTypeHeaderValue(contentType, encoding).ToString();
                return owner.File(xmlStream, mediaTypeHeaderValue, fileDownloadName);
            case string stringBody:
                contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(stringBody), contentType);
                encoding = CustomApiHelpers.GetEncoding(stringBody);
                mediaTypeHeaderValue = CustomApiHelpers.PrepareMediaTypeHeaderValue(contentType, encoding).ToString();
                return owner.File(System.Text.Encoding.UTF8.GetBytes(stringBody), mediaTypeHeaderValue, fileDownloadName);
            case Stream streamBody:
                contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(streamBody), contentType);
                encoding = CustomApiHelpers.GetEncoding(streamBody);
                mediaTypeHeaderValue = CustomApiHelpers.PrepareMediaTypeHeaderValue(contentType, encoding).ToString();
                return owner.File(streamBody, mediaTypeHeaderValue, fileDownloadName);
            case byte[] charBody:
                contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(charBody), contentType);
                encoding = CustomApiHelpers.GetEncoding(charBody);
                mediaTypeHeaderValue = CustomApiHelpers.PrepareMediaTypeHeaderValue(contentType, encoding).ToString();
                return owner.File(charBody, mediaTypeHeaderValue, fileDownloadName);
            default:
                throw new ArgumentException("Tried to provide file download but couldn't find content");
        }
    }

}