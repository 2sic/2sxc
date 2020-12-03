using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    public abstract class IntStatelessControllerBase : ControllerBase, IHasLog
    {
        protected IntStatelessControllerBase()
        {
            // ReSharper disable VirtualMemberCallInConstructor
            Log = new Log(HistoryLogName, null, $"Path: {HttpContext?.Request.GetDisplayUrl()}");
            History.Add(HistoryLogGroup, Log);
            // ReSharper restore VirtualMemberCallInConstructor
        }

        /// <inheritdoc />
        public ILog Log { get; }

        /// <summary>
        /// The group name for log entries in insights.
        /// Helps group various calls by use case. 
        /// </summary>
        protected virtual string HistoryLogGroup { get; } = "web-api";

        /// <summary>
        /// The name of the logger in insights.
        /// The inheriting class should provide the real name to be used.
        /// </summary>
        protected abstract string HistoryLogName { get; }
        
    }

}
