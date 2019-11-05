using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    internal abstract class ValidatorBase: HasLog
    {
        public string Errors = string.Empty;

        protected ValidatorBase(string logName, ILog parentLog, string initialMessage, string className) 
            : base(logName, parentLog, initialMessage, className)
        {
        }

        public bool HasErrors => Errors != string.Empty;

        /// <summary>
        /// Determine if errors exist, and return that state
        /// </summary>
        /// <returns></returns>
        protected bool BuildExceptionIfHasIssues(out HttpResponseException preparedException, string logMessage = null)
        {
            var wrapLog = Log.Call(nameof(BuildExceptionIfHasIssues));
            preparedException = HasErrors ? Http.BadRequest(Errors): null;
            if (logMessage != null) Log.Add($"{nameof(logMessage)}:{logMessage}");
            if (HasErrors) Log.Add($"Errors:{Errors}");
            wrapLog(HasErrors ? "found errors" : "all ok");
            return !HasErrors;
        }


        /// <summary>
        /// Add an error message
        /// </summary>
        /// <param name="addition"></param>
        protected void Add(string addition)
        {
            Log.Add($"Add problem to list:{addition}");
            Errors += (Errors == string.Empty ? "" : "\n") + addition;
        }
    }
}
