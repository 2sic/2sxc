using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ToSic.SexyContent.DataImportExport
{
    // TODO2tk: Improve the error handling... and validate the input values.. is a date.. is a boolean...
    public enum DataImportErrorCode
    {
        [LocalizedDescription("Unknown", typeof(DataImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        Unknown,

        [LocalizedDescription("InvalidContentType", typeof(DataImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidContentType,

        [LocalizedDescription("InvalidDocument", typeof(DataImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidDocument,

        [LocalizedDescription("InvalidRoot", typeof(DataImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidRoot,

        [LocalizedDescription("InvalidLanguage", typeof(DataImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidLanguage,

        [LocalizedDescription("InvalidValueReference", typeof(DataImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidValueReference
    }

    public class DataImportError
    {
        public string ErrorDetail { get; private set; }

        public DataImportErrorCode ErrorCode { get; private set; }

        public DataImportError(DataImportErrorCode errorCode, string errorDetail = null)
        {
            this.ErrorCode = errorCode;
            this.ErrorDetail = errorDetail;
        }
    }
}