using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using ToSic.Sys.Logging;

namespace ToSic.Sxc.Oqt.Server.Tests.WebApi;

public class OqtLogWebApiTests
{
    [Fact]
    public void InternalControllers_DeclareEachRequestFilterOnce()
    {
        var controllers = typeof(OqtLogWebApiAttribute).Assembly.GetTypes()
            .Where(type => type.IsPublic && !type.IsAbstract
                && type.Namespace?.StartsWith("ToSic.Sxc.Oqt.Server.WebApi.", StringComparison.Ordinal) == true
                && typeof(ControllerBase).IsAssignableFrom(type))
            .ToArray();

        NotEmpty(controllers);
        foreach (var controller in controllers)
        {
            Single(controller.GetCustomAttributes<OqtLogWebApiAttribute>(inherit: true));
            Single(controller.GetCustomAttributes<OqtInitializeAliasAttribute>(inherit: true));
            Single(controller.GetCustomAttributes<NoContentForEmptyActionAttribute>(inherit: true));
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Execute_RecordsOneCompletedInsightsTimer(bool actionThrows)
    {
        const string historyGroup = "web-api.test";
        var store = new FakeLogStore();
        using var services = new ServiceCollection().AddSingleton<ILogStore>(store).BuildServiceProvider();
        var log = new Log("Api.Test");
        var executing = CreateExecutingContext(services, log);
        var error = new InvalidOperationException("Test action failed");
        var filter = new OqtLogWebApiAttribute(historyGroup);

        var failure = await Record.ExceptionAsync(() => filter.OnActionExecutionAsync(executing, () =>
            actionThrows
                ? Task.FromException<ActionExecutedContext>(error)
                : Task.FromResult(new ActionExecutedContext(executing, [], executing.Controller))));

        Same(actionThrows ? error : null, failure);
        Equal((historyGroup, (ILog)log), Single(store.Entries));
        var timer = Single(log.Entries, entry => entry.WrapOpen);
        True(timer.WrapOpenWasClosed);
    }

    [Fact]
    public async Task Execute_LogsOnce_WhenControllerAndActionBothDeclareTheFilter()
    {
        const string controllerGroup = "web-api.controller";
        var store = new FakeLogStore();
        using var services = new ServiceCollection().AddSingleton<ILogStore>(store).BuildServiceProvider();
        var log = new Log("Api.Test");
        var executing = CreateExecutingContext(services, log);
        var controllerFilter = new OqtLogWebApiAttribute(controllerGroup);
        var actionFilter = new OqtLogWebApiAttribute("web-api.action");
        var actionCalls = 0;

        await controllerFilter.OnActionExecutionAsync(executing, async () =>
        {
            await actionFilter.OnActionExecutionAsync(executing, () =>
            {
                actionCalls++;
                return Task.FromResult(new ActionExecutedContext(executing, [], executing.Controller));
            });
            return new ActionExecutedContext(executing, [], executing.Controller);
        });

        Equal(1, actionCalls);
        Equal((controllerGroup, (ILog)log), Single(store.Entries));
        True(Single(log.Entries, entry => entry.WrapOpen).WrapOpenWasClosed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Execute_RunsActionAndClosesTimer_WhenInsightsStoreThrows(bool actionThrows)
    {
        var store = new FakeLogStore { FailOnAdd = true };
        using var services = new ServiceCollection().AddSingleton<ILogStore>(store).BuildServiceProvider();
        var log = new Log("Api.Test");
        var executing = CreateExecutingContext(services, log);
        var filter = new OqtLogWebApiAttribute();
        var actionError = new InvalidOperationException("Test action failed");
        var actionCalls = 0;

        var failure = await Record.ExceptionAsync(() => filter.OnActionExecutionAsync(executing, () =>
        {
            actionCalls++;
            return actionThrows
                ? Task.FromException<ActionExecutedContext>(actionError)
                : Task.FromResult(new ActionExecutedContext(executing, [], executing.Controller));
        }));

        Equal(1, actionCalls);
        Same(actionThrows ? actionError : null, failure);
        True(Single(log.Entries, entry => entry.WrapOpen).WrapOpenWasClosed);
    }

    [Fact]
    public async Task Execute_PreservesOriginalActionException_WhenTimerCompletionThrows()
    {
        using var loggerFactory = new FailingLoggerFactory();
        var log = new MelLogFactory(loggerFactory).Create("Api.Test", null, new());
        using var services = new ServiceCollection().AddSingleton<ILogStore>(new MelLogStore()).BuildServiceProvider();
        var executing = CreateExecutingContext(services, log);
        var filter = new OqtLogWebApiAttribute();
        var actionError = new InvalidOperationException("Test action failed");

        var failure = await Record.ExceptionAsync(() => filter.OnActionExecutionAsync(executing,
            () => Task.FromException<ActionExecutedContext>(actionError)));

        Same(actionError, failure);
        Equal(1, loggerFactory.LogAttempts);
    }

    private static ActionExecutingContext CreateExecutingContext(IServiceProvider services, ILog log)
    {
        var http = new DefaultHttpContext { RequestServices = services };
        http.Request.Scheme = "https";
        http.Request.Host = new("example.invalid");
        var action = new ActionContext(http, new RouteData(), new ActionDescriptor(), new ModelStateDictionary());
        return new(action, [], new Dictionary<string, object?>(), new TestController(log));
    }

    private sealed class TestController(ILog log) : ControllerBase, IHasLog
    {
        public ILog Log { get; } = log;
    }

    private sealed class FakeLogStore : ILogStore
    {
        public List<(string Group, ILog Log)> Entries { get; } = [];
        public bool FailOnAdd { get; init; }

        public LogStoreEntry? Add(string segment, ILog log)
        {
            if (FailOnAdd)
                throw new InvalidOperationException("Test Insights store failed");

            Entries.Add((segment, log));
            return null;
        }

        public LogStoreEntry? ForceAdd(string key, ILog log) => throw new NotSupportedException();
    }

    private sealed class FailingLoggerFactory : ILoggerFactory, ILogger
    {
        public int LogAttempts { get; private set; }

        public ILogger CreateLogger(string categoryName) => this;
        public void AddProvider(ILoggerProvider provider) => throw new NotSupportedException();
        public void Dispose() { }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            LogAttempts++;
            throw new InvalidOperationException("Test timer logging failed");
        }
    }
}
