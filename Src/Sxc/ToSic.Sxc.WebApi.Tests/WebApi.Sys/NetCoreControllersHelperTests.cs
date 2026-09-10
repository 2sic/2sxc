#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToSic.Sxc.WebApi.Sys;
using static Xunit.Assert;

namespace Tests.ToSic.ToSxc.WebApi.WebApi.Sys;

public class NetCoreControllersHelperTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OnActionExecutionAsync_ClearsAmbientScope_AfterShortCircuitOrCancellation(bool cancel)
    {
        using var scopes = new ScopeTrackingProvider();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(scopes));
        using var services = new ServiceCollection()
            .AddSingleton<ILogStore, FakeLogStore>()
            .AddSingleton<ILoggerFactory>(loggerFactory)
            .BuildServiceProvider();
        var controller = new TestController();
        var helper = new NetCoreControllersHelper(controller);
        var descriptor = new ControllerActionDescriptor
        {
            ControllerName = "Test",
            ActionName = "Action",
            MethodInfo = typeof(NetCoreControllersHelperTests).GetMethod(nameof(Action))!,
        };
        var actionContext = new ActionContext(
            new DefaultHttpContext { RequestServices = services },
            new RouteData(),
            descriptor);
        var filters = new List<IFilterMetadata>();
        var executing = new ActionExecutingContext(actionContext, filters, new Dictionary<string, object?>(), controller);

        async Task<ActionExecutedContext> Next()
        {
            True(scopes.HasExecutionScope);
            await Task.Yield();
            if (cancel)
                throw new OperationCanceledException();
            return new(actionContext, filters, controller) { Canceled = true };
        }

        if (cancel)
            await ThrowsAsync<OperationCanceledException>(() => helper.OnActionExecutionAsync(executing, Next, "test"));
        else
            await helper.OnActionExecutionAsync(executing, Next, "test");

        False(scopes.HasExecutionScope);
    }

    public static Task Action() => Task.CompletedTask;

    private sealed class TestController : ControllerBase, IHasLog
    {
        public ILog Log { get; } = new Log("Tst.Ctrl");
    }

    private sealed class FakeLogStore : ILogStore
    {
        public LogStoreEntry? Add(string segment, ILog log) => null;

        public LogStoreEntry? ForceAdd(string key, ILog log) => null;
    }

    private sealed class ScopeTrackingProvider : ILoggerProvider, ISupportExternalScope
    {
        private IExternalScopeProvider _scopes = new LoggerExternalScopeProvider();

        public bool HasExecutionScope
        {
            get
            {
                var found = false;
                _scopes.ForEachScope((scope, _) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object>> values
                        && values.Any(pair => pair.Key == LogExecution.AmbientLogIdKey))
                        found = true;
                }, state: 0);
                return found;
            }
        }

        public ILogger CreateLogger(string categoryName) => new ScopeTrackingLogger(this);

        public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopes = scopeProvider;

        public void Dispose() { }

        private sealed class ScopeTrackingLogger(ScopeTrackingProvider provider) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) where TState : notnull
                => provider._scopes.Push(state);

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter) { }
        }
    }
}
#endif
