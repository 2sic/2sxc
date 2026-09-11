using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sys.Logging;
using ToSic.Sys.Run.Startup;

namespace ToSic.Sxc.Oqt.Controllers;

[CollectionDefinition(nameof(OqtControllerLoggingTests), DisableParallelization = true)]
public sealed class OqtControllerLoggingTestCollection;

[Collection(nameof(OqtControllerLoggingTests))]
public class OqtControllerLoggingTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task OnActionExecutionAsync_RunsLegacyHooksInsideScope_AndHonorsShortCircuit(bool typed, bool shortCircuit)
    {
        using var services = new ServiceCollection().AddSysCoreLogging().BuildServiceProvider();
        var factory = services.GetRequiredService<ILoggerFactory>();
        var logger = factory.CreateLogger(MicrosoftLoggerEventSink.Category);
        var store = services.GetRequiredService<ILogStoreLive>();
        LogEventBridge.SetSink(new MicrosoftLoggerEventSink(factory));
        try
        {
            store.Configure("ILogger", bridgeEnabled: true);
            var calls = new List<string>();
            var result = new OkObjectResult("custom response");
            void Before(ActionExecutingContext context)
            {
                calls.Add("before");
                logger.LogInformation("legacy before hook");
                if (shortCircuit)
                    context.Result = result;
            }
            void After(ActionExecutedContext context)
            {
                calls.Add("after");
                logger.LogInformation("legacy after hook");
                Equal(shortCircuit, context.Canceled);
                Same(result, context.Result);
            }

            OqtControllerBase controller = typed ? new LegacyTypedApi(Before, After) : new LegacyApi12(Before, After);
            IsAssignableFrom<IActionFilter>(controller);
            var descriptor = new ControllerActionDescriptor
            {
                ControllerName = "Test", ActionName = "Action",
                MethodInfo = typeof(OqtControllerLoggingTests).GetMethod(nameof(Action))!,
            };
            var actionContext = new ActionContext(new DefaultHttpContext { RequestServices = services }, new RouteData(), descriptor);
            var filters = new List<IFilterMetadata>();
            var executing = new ActionExecutingContext(actionContext, filters, new Dictionary<string, object?>(), controller);
            var previousActivity = Activity.Current;

            await controller.OnActionExecutionAsync(executing, async () =>
            {
                await Task.Yield();
                calls.Add("action");
                return new ActionExecutedContext(actionContext, filters, controller) { Result = result };
            });
            logger.LogInformation("outside request");

            Equal(shortCircuit ? new[] { "before" } : new[] { "before", "action", "after" }, calls);
            if (shortCircuit)
                Same(result, executing.Result);
            var entries = store.Snapshot(controller.Log)!.Entries;
            Contains(entries, e => e.Message == "legacy before hook");
            if (shortCircuit)
                DoesNotContain(entries, e => e.Message == "legacy after hook");
            else
                Contains(entries, e => e.Message == "legacy after hook");
            DoesNotContain(entries, e => e.Message == "outside request");
            Same(previousActivity, Activity.Current);
        }
        finally
        {
            LogEventBridge.SetSink(null);
        }
    }

    public static object Action() => new();

    // These overrides also keep the existing custom-controller signatures under compilation coverage.
    private sealed class LegacyTypedApi(Action<ActionExecutingContext> before, Action<ActionExecutedContext> after) : global::Custom.Hybrid.ApiTyped
    {
        public override void OnActionExecuting(ActionExecutingContext context) => before(context);
        public override void OnActionExecuted(ActionExecutedContext context) => after(context);
    }

    private sealed class LegacyApi12(Action<ActionExecutingContext> before, Action<ActionExecutedContext> after) : global::Custom.Hybrid.Api12
    {
        public override void OnActionExecuting(ActionExecutingContext context) => before(context);
        public override void OnActionExecuted(ActionExecutedContext context) => after(context);
    }
}
