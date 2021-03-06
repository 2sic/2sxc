﻿using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.WebApi
{
    public class CustomApiHelpers
    {
        public static string FileParamsInitialCheck(string dontRelyOnParameterOrder, bool? download, string virtualPath,
            string fileDownloadName, object contents)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, nameof(File), nameof(download) + "," + nameof(virtualPath) + "," + nameof(fileDownloadName) + "," + nameof(contents));

            // Check initial conflicting values
            CheckInitialConflictingValues(virtualPath, contents);

            return CheckForceDownload(download, virtualPath, fileDownloadName);
        }

        /**
         * check initial conflicting value
         */
        public static void CheckInitialConflictingValues(string virtualPath, object contents)
        {
            var contentCount = (contents != null ? 1 : 0) + (virtualPath != null ? 1 : 0);
            if (contentCount == 0)
                throw new ArgumentException("None of the provided parameters give content for the file to return.");
            if (contentCount > 1)
                throw new ArgumentException(
                    $"Multiple file setting properties like '{nameof(contents)}' or '{nameof(virtualPath)}' have a value - only one can be provided.");
        }

        public static string CheckForceDownload(bool? download, string virtualPath, string fileDownloadName)
        {
            // Set reallyForceDownload based on forceDownload and file name
            var reallyForceDownload = download == true || !string.IsNullOrWhiteSpace(fileDownloadName);

            // check if we should force a download, but maybe fileDownloadName is empty
            if (reallyForceDownload && string.IsNullOrWhiteSpace(fileDownloadName))
            {
                // try to guess name based on virtualPath name
                fileDownloadName = !string.IsNullOrWhiteSpace(virtualPath) ? Path.GetFileName(virtualPath) : null;
                if (string.IsNullOrWhiteSpace(fileDownloadName))
                    throw new HttpExceptionAbstraction(HttpStatusCode.NotFound,
                        $"Can't force download without a {nameof(fileDownloadName)} or a real {nameof(virtualPath)}",
                        "Not Found");
            }

            return fileDownloadName;
        }

        public static string XmlContentTypeFromContent(bool isValidXml, string contentType)
        {
            return isValidXml ? "text/xml" : contentType;
        }

        //public static bool IsValidXml(string xml)
        //{
        //    try
        //    {
        //        XDocument.Parse(xml);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public static bool IsValidXml(string xml)
        {
            var stringReader = new StringReader(xml);
            var disposable = XmlReader.Create(stringReader, _settings);
            return IsMinimallyValidXml(disposable);
        }

        public static bool IsValidXml(Stream stream)
        {
            var disposable = XmlReader.Create(stream, _settings);
            var isValidXml = IsMinimallyValidXml(disposable);
            stream.Position = 0; // stream was used for validation, so prepare it for new use
            return isValidXml;
        }

        public static bool IsValidXml(byte[] body)
        {
            // load the data into a memory stream
            using (var stream = new MemoryStream(body))
            {
                return IsValidXml(stream);
            }
        }

        public static bool IsMinimallyValidXml(XmlReader disposable)
        {
            using (var xmlReader = disposable)
            {
                try
                {
                    while (xmlReader.Read())
                    {
                        ; // Intentionally left blank.
                    }
                    return true;
                }
                catch (XmlException)
                {
                    return  false;
                }
            }
        }

        private static readonly XmlReaderSettings _settings = new XmlReaderSettings
        {
            CheckCharacters = true,
            ConformanceLevel = ConformanceLevel.Document,
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            ValidationFlags = XmlSchemaValidationFlags.None,
            ValidationType = ValidationType.None,
        };
    }
}
