using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.WebApi.Validation
{
    internal abstract class ValidatorBase: HasLog
    {
        public string Errors = string.Empty;

        protected ValidatorBase(string logName, ILog parentLog, string initialMessage, string className) 
            : base(logName, parentLog, initialMessage, className)
        {
        }

        private bool HasErrors => Errors != string.Empty;

        /// <summary>
        /// Determine if errors exist, and return that state
        /// </summary>
        /// <returns></returns>
        protected bool BuildExceptionIfHasIssues(out HttpExceptionAbstraction preparedException, string logMessage = null)
        {
            var wrapLog = Log.Call();
            preparedException = HasErrors ? HttpException.BadRequest(Errors): null;
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
