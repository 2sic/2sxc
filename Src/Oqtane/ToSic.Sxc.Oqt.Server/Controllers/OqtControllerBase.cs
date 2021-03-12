using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared.Dev;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    /// <summary>
    /// Api controllers normally should inherit ControllerBase but we have a special case of inhering from Controller.
    /// It is because our custom dynamic 2sxc app api controllers (without constructor), depends on event OnActionExecuting
    /// to provide dependencies (without DI in constructor).
    /// </summary>
    public abstract class OqtControllerBase : Controller, IHasLog
    {
        protected OqtControllerBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            // todo: redesign so it works - in .net core the HttpContext isn't ready in the constructor
            Log = new Log(HistoryLogName, null, $"Path: {HttpContext?.Request.GetDisplayUrl()}");
            // ReSharper disable once VirtualMemberCallInConstructor
            History.Add(HistoryLogGroup, Log);
            // todo: get this to work
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

        #region Extend Time so Web Server doesn't time out - not really implemented ATM

        protected void PreventServerTimeout300() => WipConstants.DontDoAnythingImplementLater();

        #endregion
    }

}
