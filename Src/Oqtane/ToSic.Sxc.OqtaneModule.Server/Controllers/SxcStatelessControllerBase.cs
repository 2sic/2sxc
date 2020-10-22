using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Models;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.OqtaneModule.Server.Run;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.OqtaneModule.Server.Controllers
{
    public abstract class SxcStatelessControllerBase : ControllerBase, IHasLog
    {
        protected SxcStatelessControllerBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            // todo: redesign so it works - in .net core the HttpContext isn't ready in the constructor
            Log = new Log(HistoryLogName, null, $"Path: {HttpContext?.Request.GetDisplayUrl()}");
            //TimerWrapLog = Log.Call(message: "timer", useTimer: true);
            // ReSharper disable once VirtualMemberCallInConstructor
            History.Add(HistoryLogGroup, Log);
            // register for dispose / stopping the timer at the end
            _logWrapper = new LogWrapper(Log);
            // todo: get this to work
            // ControllerContext.HttpContext.Response.RegisterForDispose(_logWrapper);
        }

        #region Dummy Content-Block, as still stateless

        protected IBlock NoBlock => null;

        #endregion

        /// <inheritdoc />
        public ILog Log { get; }

        private readonly LogWrapper _logWrapper;

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

        //protected void Dispose(bool disposing)
        //{
        //    TimerWrapLog(null);
        //    base.Dispose(disposing);
        //}

        public OqtaneUser GetUser()
        {
            var user = new User { IsAuthenticated = User.Identity.IsAuthenticated, Username = "", UserId = -1, Roles = "" };
            if (!user.IsAuthenticated) return new OqtaneUser(user);

            user.Username = User.Identity.Name;
            user.UserId = int.Parse(User.Claims.First(item => item.Type == ClaimTypes.PrimarySid).Value);
            var roles = User.Claims.Where(item => item.Type == ClaimTypes.Role).Aggregate("", (current, claim) => current + (claim.Value + ";"));
            if (roles != "") roles = ";" + roles;
            user.Roles = roles;
            return new OqtaneUser(user);
        }
    }

    internal class LogWrapper : IDisposable
    {
        private readonly Action<string> _timerWrapLog;

        internal LogWrapper(ILog log) => _timerWrapLog = log.Call(message: "timer", useTimer: true);
        public void Dispose() => _timerWrapLog(null);
    }
}
