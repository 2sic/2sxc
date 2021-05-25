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
        public AppApiActionInvoker()
        {
            Log = new Log(HistoryLogName, null, "AppApiActionInvoker");
            History.Add(HistoryLogGroup, Log);
        }

        public ILog Log { get; }
        protected string HistoryLogGroup { get; } = "app-api";
        protected static string HistoryLogName => "Invoker";

        public async Task Invoke(ActionContext actionContext)
        {
            var actionInvokerFactory = actionContext.HttpContext.RequestServices.GetRequiredService<IActionInvokerFactory>();

            var actionInvoker = actionInvokerFactory.CreateInvoker(actionContext);

            Log.Add($"invoke app api action");
            await actionInvoker.InvokeAsync();
        }
    }
}
