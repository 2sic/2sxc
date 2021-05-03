using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Oqtane.Shared;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Oqt.Server.Plumbing;
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
            Log = new Log(HistoryLogName, null, $"OqtControllerBase");
            // ReSharper disable once VirtualMemberCallInConstructor
            History.Add(HistoryLogGroup, Log);
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

        protected SiteState SiteState { get; private set; }


        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Log.Add($"Url: {context.HttpContext.Request.GetDisplayUrl()}");

            base.OnActionExecuting(context);

            var serviceProvider = context.HttpContext.RequestServices;
            SiteState = serviceProvider.Build<SiteState>();

            // background processes can pass in an alias using the SiteState service
            if (SiteState.Alias != null) return;

            //var request = context.HttpContext.Request;
            //var host = $"{request.Host}";

            //var siteId = -1;

            //// get siteId identifier based on request
            //// TODO: REMOVE THIS CODE AS SOON AS THE ui DOESN'T USE IDs ANY MORE - wip ca. mid May 2021
            //var segments = request.Path.Value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            //if (segments != null && segments.Length > 1 && (segments[1] == "api" || segments[1] == "pages") && segments[0] != "~" && int.TryParse(segments[0], out var areaId))
            //    siteId = areaId; // 2sxc UI sends siteId instead of areaId that is common in Oqtane.

            // New: Get the Site Alias based on the URL
            //var aliasRepositoryLazy = serviceProvider.Build<Lazy<IAliasRepository>>();
            //if (siteId == -1)
                serviceProvider.Build<SiteStateInitializer>().InitIfEmpty();
            //SiteStateInitializer.InitIfEmpty(SiteState, context.HttpContext, aliasRepositoryLazy);
            //// TODO: REMOVE THIS CODE AS SOON AS THE ui DOESN'T USE IDs ANY MORE - wip ca. mid May 2021
            //else
            //{
            //    var aliases = aliasRepositoryLazy.Value.GetAliases().ToList(); // cached
            //    SiteState.Alias = aliases.OrderBy(a => a.Name)
            //        .FirstOrDefault(a => a.SiteId == siteId && a.Name.StartsWith(host));

            //}
        }

        #region Extend Time so Web Server doesn't time out - not really implemented ATM

        protected void PreventServerTimeout300() => WipConstants.DontDoAnythingImplementLater();

        #endregion


    }


}
