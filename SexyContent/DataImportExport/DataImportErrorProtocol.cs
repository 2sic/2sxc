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

        private List<DataImportError> errors = new List<DataImportError>();

        public int ErrorCount
        {
            get { return errors.Count; }
        }

        public void AppendError(DataImportErrorCode errorCode, string errorDetail = null)
        {
            errors.Add(new DataImportError(errorCode, errorDetail));
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