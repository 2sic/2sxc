using System;
using System.Collections.Generic;

namespace ToSic.SexyContent.DataImportExport
{
    public class ImportErrorProtocol : IEnumerable<ImportError>
    {
        public ImportError this[int index]
        {
            get { return errors[index]; }
        }

        public List<ImportError> Errors
        {
            get { return errors; }
        }
        private List<ImportError> errors = new List<ImportError>();

        public int ErrorCount
        {
            get { return errors.Count; }
        }

        public void AppendError(ImportErrorCode errorCode, string errorDetail = null, int? lineNumber = null, string lineDetail = null)
        {
            errors.Add(new ImportError(errorCode, errorDetail, lineNumber, lineDetail));
        }

        public IEnumerator<ImportError> GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}