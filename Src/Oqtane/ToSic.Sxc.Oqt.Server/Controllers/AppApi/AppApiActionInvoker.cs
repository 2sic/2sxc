using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

// TODO: @STV - PLS EXPLAIN what this does / what it's for
public class AppApiActionInvoker : IHasLog
{
    public AppApiActionInvoker(ILogStore logStore)
    {
        Log = new Log(HistoryLogName, null, "AppApiActionInvoker");
        logStore.Add(HistoryLogGroup, Log);
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