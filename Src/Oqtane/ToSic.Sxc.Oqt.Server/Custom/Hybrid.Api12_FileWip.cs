using System;
using System.IO;
using ToSic.Eav;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12
    {
        #region Experimental


        public dynamic File(string dontRelyOnParameterOrder = Parameters.Protector,
            // Important: the second parameter should _not_ be a string, otherwise the signature looks the same as the built-in File(...) method
            bool? download = null,
            string virtualPath = null, // important: this is the virtualPath, but it should not have the same name, to not confuse the compiler with same sounding param names
            string contentType = null,
            string fileDownloadName = null,
            object contents = null // can be stream, string or byte[]
            )
        {
            fileDownloadName = CustomApiHelpers.FileParamsInitialCheck(dontRelyOnParameterOrder, download, virtualPath, fileDownloadName, contents);

            // Try to figure out file mime type as needed
            if (string.IsNullOrWhiteSpace(contentType))
                contentType = ContentFileHelper.GetMimeType(fileDownloadName ?? virtualPath);
            
            // check if this may just be a call to the built in file, which has two strings
            // this can only be possible if only the virtualPath and contentType were set
            if (!string.IsNullOrWhiteSpace(virtualPath))
                return base.File(virtualPath, contentType, fileDownloadName);

            switch (contents)
            {
                case string stringBody:
                    contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(stringBody), contentType);
                    return base.File( System.Text.Encoding.UTF8.GetBytes(stringBody), contentType, fileDownloadName);
                case Stream streamBody:
                    contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(streamBody), contentType);
                    return base.File(streamBody, contentType, fileDownloadName);
                case byte[] charBody:
                    contentType = CustomApiHelpers.XmlContentTypeFromContent(CustomApiHelpers.IsValidXml(charBody), contentType);
                    return base.File(charBody, contentType, fileDownloadName);
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
