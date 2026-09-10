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
    [Fact]
    public async Task ExecuteActionFilterAsync_ClearsExecution_AfterCancellation()
    {
        using var source = new ActivitySource("DnnLogWebApiTests");
        using var listener = new ActivityListener
        {
            ShouldListenTo = candidate => candidate == source,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
        };
        ActivitySource.AddActivityListener(listener);
        using var loggerFactory = LoggerFactory.Create(_ => { });
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

        async Task<HttpResponseMessage> Next()
        {
            NotNull(Activity.Current);
            await Task.Yield();
            throw new OperationCanceledException();
        }

        await ThrowsAsync<OperationCanceledException>(() => filter.ExecuteActionFilterAsync(actionContext,
            CancellationToken.None, Next));

        Null(Activity.Current);
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
}
