using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public class DataImportErrorProtocol : IEnumerable<DataImportError>
    {
        public DataImportError this[int index]
        {
            get { return errors[index]; }
        }

        public List<DataImportError> Errors
        {
            get { return errors; }
        }
        private List<DataImportError> errors = new List<DataImportError>();

        public int ErrorCount
        {
            get { return errors.Count; }
        }

        public void AppendError(DataImportErrorCode errorCode, string errorDetail = null, int? lineNumber = null, string lineDetail = null)
        {
            errors.Add(new DataImportError(errorCode, errorDetail, lineNumber, lineDetail));
        }

        public IEnumerator<DataImportError> GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}