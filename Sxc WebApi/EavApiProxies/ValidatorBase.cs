using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    internal abstract class ValidatorBase: HasLog
    {
        public string Errors = string.Empty;

        protected ValidatorBase(string logName, Log parentLog = null, string initialMessage = null) : base(logName, parentLog, initialMessage)
        {
        }

        public bool HasErrors => Errors != string.Empty;

        /// <summary>
        /// Determine if errors exist, and return that state
        /// </summary>
        /// <returns></returns>
        protected bool BuildTrueIfOk(out HttpResponseException preparedException, string logMessage = null)
        {
            preparedException = HasErrors ? Http.BadRequest(Errors): null;
            Log.Add(HasErrors ? "found errors: " + Errors : "no errors found");
            if (logMessage != null) Log.Add(logMessage);
            return !HasErrors;
        }


        /// <summary>
        /// Add an error message
        /// </summary>
        /// <param name="addition"></param>
        protected void Add(string addition) => Errors += (Errors == string.Empty ? "" : "\n") + addition;

    }
}
