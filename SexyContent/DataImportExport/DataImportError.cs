using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ToSic.SexyContent.DataImportExport
{
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
        InvalidValueReference,

        [LocalizedDescription("InvalidValueFormat", typeof(DataImportErrorCode), "ToSic.SexyContent.SexyContent.DataImportExport")]
        InvalidValueFormat
    }

    public class DataImportError
    {
        public int? LineNumber { get; private set; }

        public string LineDetail { get; private set; }

        public string ErrorDetail { get; private set; }

        public DataImportErrorCode ErrorCode { get; private set; }

        public DataImportError(DataImportErrorCode errorCode, string errorDetail = null, int? lineNumber = null, string lineDetail = null)
        {
            this.ErrorCode = errorCode;
            this.ErrorDetail = errorDetail;
            this.LineNumber = lineNumber;
            this.LineDetail = lineDetail;
        }
    }
}