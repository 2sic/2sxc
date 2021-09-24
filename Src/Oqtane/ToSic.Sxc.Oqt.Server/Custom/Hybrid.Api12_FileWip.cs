using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Xml;
using Microsoft.Net.Http.Headers;
using ToSic.Eav;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12
    {
        #region Experimental

        public dynamic File(string noParamOrder = Parameters.Protector,
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
                contentType = ContentFileHelper.GetMimeType(fileDownloadName ?? virtualPath);

            // check if this may just be a call to the built in file, which has two strings
            // this can only be possible if only the virtualPath and contentType were set
            if (!string.IsNullOrWhiteSpace(virtualPath))
                return base.File(virtualPath, contentType, fileDownloadName);

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
                    return base.File(xmlStream, mediaTypeHeaderValue, fileDownloadName);
                case string stringBody:
                    contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(stringBody), contentType);
                    encoding = CustomApiHelpers.GetEncoding(stringBody);
                    mediaTypeHeaderValue = CustomApiHelpers.PrepareMediaTypeHeaderValue(contentType, encoding).ToString();
                    return base.File(System.Text.Encoding.UTF8.GetBytes(stringBody), mediaTypeHeaderValue, fileDownloadName);
                case Stream streamBody:
                    contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(streamBody), contentType);
                    encoding = CustomApiHelpers.GetEncoding(streamBody);
                    mediaTypeHeaderValue = CustomApiHelpers.PrepareMediaTypeHeaderValue(contentType, encoding).ToString();
                    return base.File(streamBody, mediaTypeHeaderValue, fileDownloadName);
                case byte[] charBody:
                    contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(charBody), contentType);
                    encoding = CustomApiHelpers.GetEncoding(charBody);
                    mediaTypeHeaderValue = CustomApiHelpers.PrepareMediaTypeHeaderValue(contentType, encoding).ToString();
                    return base.File(charBody, mediaTypeHeaderValue, fileDownloadName);
                default:
                    throw new ArgumentException("Tried to provide file download but couldn't find content");
            }
        }

        private void Test()
        {
            var x = base.Content("");
            var y = base.Ok();
            var z = base.Redirect("todo");
        }

        #endregion

    }
}
