using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;


/// <summary>
/// The AppApiActionInvoker class is a custom action invoker for an ASP.NET Core application, for use within "AppApi". 
/// An action invoker in ASP.NET Core is responsible for executing action methods within controllers based on incoming HTTP requests.
/// </summary>
internal class AppApiActionInvoker : IHasLog
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