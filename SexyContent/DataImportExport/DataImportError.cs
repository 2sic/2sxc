using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ToSic.SexyContent.DataImportExport
{
    public enum DataImportErrorCode
    {
        // TODO2tk: Translate the enumeration values like described on 
        // http://stackoverflow.com/questions/17380900/enum-localization

        [Description("Unknown error.")]
        Unknown,

        [Description("Content type not found.")]
        InvalidContentType,

        [Description("Cannot load document. It is not a valid XML.")]
        InvalidDocument,

        [Description("Cannot load document. It has not the content expected.")]
        InvalidRoot,

        [Description("Language is not supported.")]
        InvalidLanguage,

        [Description("Value referenced is not available.")]
        InvalidValueReference,
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