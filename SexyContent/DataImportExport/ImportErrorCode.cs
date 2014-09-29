using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum ImportErrorCode
    {
        [LocalizedDescription("Unknown", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        Unknown,

        [LocalizedDescription("InvalidContentType", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidContentType,

        [LocalizedDescription("InvalidDocument", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidDocument,

        [LocalizedDescription("InvalidRoot", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidRoot,

        [LocalizedDescription("InvalidLanguage", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidLanguage,

        [LocalizedDescription("MissingElementLanguage", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        MissingElementLanguage,

        [LocalizedDescription("InvalidValueReference", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidValueReference,

        [LocalizedDescription("InvalidValueReferenceProtection", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidValueReferenceProtection,

        [LocalizedDescription("InvalidValueFormat", typeof(ImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidValueFormat,
    }
}