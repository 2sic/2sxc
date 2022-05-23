using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public class AppApiActionInvoker : IHasLog
    {
        public AppApiActionInvoker(LogHistory logHistory)
        {
            Log = new Log(HistoryLogName, null, "AppApiActionInvoker");
            logHistory.Add(HistoryLogGroup, Log);
        }

        public ILog Log { get; }
        protected string HistoryLogGroup => "app-api";
        protected static string HistoryLogName => "Invoker";

        public async Task Invoke(ActionContext actionContext)
        {
            var actionInvokerFactory = actionContext.HttpContext.RequestServices.GetRequiredService<IActionInvokerFactory>();

            var actionInvoker = actionInvokerFactory.CreateInvoker(actionContext);

            Log.A($"invoke app api action");
            await actionInvoker.InvokeAsync();
        }
    }
}
