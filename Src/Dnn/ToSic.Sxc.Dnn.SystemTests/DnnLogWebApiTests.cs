using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Extensions.Logging;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sys.Logging;

namespace ToSic.Sxc.Dnn;

public class DnnLogWebApiTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task ExecuteActionFilterAsync_ClearsExecution_AfterCancellationOrShortCircuit(bool cancel, bool listen)
    {
        using var source = new ActivitySource("DnnLogWebApiTests");
        using var listener = new ActivityListener
        {
            ShouldListenTo = candidate => listen && candidate == source,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
        };
        ActivitySource.AddActivityListener(listener);
        var scopes = new ScopeTrackingProvider();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(scopes));
        var filter = new TestDnnLogWebApi(loggerFactory.CreateLogger("test"), source);
        var controller = new TestController();
        var controllerContext = new HttpControllerContext
        {
            Controller = controller,
            Request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/api/test"),
        };
        var actionDescriptor = new ReflectedHttpActionDescriptor(
            new HttpControllerDescriptor { ControllerType = typeof(TestController), ControllerName = "Test" },
            typeof(TestController).GetMethod(nameof(TestController.Action))!);
        var actionContext = new HttpActionContext(controllerContext, actionDescriptor);
        var previousActivity = Activity.Current;
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden);

        async Task<HttpResponseMessage> Next()
        {
            True(scopes.HasExecutionScope);
            if (listen)
                NotNull(Activity.Current);
            await Task.Yield();
            if (cancel)
                throw new OperationCanceledException();
            return response;
        }

        if (cancel)
            await ThrowsAsync<OperationCanceledException>(() => filter.ExecuteActionFilterAsync(actionContext,
                CancellationToken.None, Next));
        else
            Same(response, await filter.ExecuteActionFilterAsync(actionContext, CancellationToken.None, Next));

        False(scopes.HasExecutionScope);
        Same(previousActivity, Activity.Current);
    }

    [Fact]
    public async Task ExecuteActionFilterAsync_PropagatesException_ForDnnExceptionFilter()
    {
        var expected = new InvalidOperationException("Expected app API failure.");
        var controllerContext = new HttpControllerContext
        {
            Controller = new TestController(),
            Request = new(HttpMethod.Get, "https://example.test/api/test"),
        };
        var actionDescriptor = new ReflectedHttpActionDescriptor(
            new HttpControllerDescriptor { ControllerType = typeof(TestController), ControllerName = "Test" },
            typeof(TestController).GetMethod(nameof(TestController.Action))!);
        var actionContext = new HttpActionContext(controllerContext, actionDescriptor);
        var filter = new DnnLogWebApi();

        var exception = await ThrowsAsync<InvalidOperationException>(() => filter.ExecuteActionFilterAsync(
            actionContext,
            CancellationToken.None,
            () => Task.FromException<HttpResponseMessage>(expected)));

        Same(expected, exception);
    }

    private sealed class TestDnnLogWebApi(ILogger logger, ActivitySource source) : DnnLogWebApi
    {
        protected override IDisposable? BeginExecution(HttpActionContext actionContext)
            => logger.BeginExecution(new global::ToSic.Sys.Logging.Log("Tst.Dnn"), source, "Test.Action");
    }

    private sealed class TestController : global::System.Web.Http.ApiController
    {
        public HttpResponseMessage Action() => new(HttpStatusCode.OK);
    }

    private sealed class ScopeTrackingProvider : ILoggerProvider
    {
        private readonly IExternalScopeProvider _scopes = new LoggerExternalScopeProvider();
        public bool HasExecutionScope
        {
            get
            {
                var found = false;
                _scopes.ForEachScope((scope, _) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object>> values)
                        found |= values.Any(pair => pair.Key == LogExecution.AmbientLogIdKey);
                }, 0);
                return found;
            }
        }

        public ILogger CreateLogger(string categoryName) => new ScopeLogger(_scopes);
        public void Dispose() { }

        private sealed class ScopeLogger(IExternalScopeProvider scopes) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) where TState : notnull => scopes.Push(state);
            public bool IsEnabled(LogLevel level) => true;
            public void Log<TState>(LogLevel level, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
        }
    }
}
