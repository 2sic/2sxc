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
        var http = new DefaultHttpContext { RequestServices = services };
        http.Request.Scheme = "https";
        http.Request.Host = new("example.invalid");
        var action = new ActionContext(http, new RouteData(), new ActionDescriptor(), new ModelStateDictionary());
        var log = new Log("Api.Test");
        var controller = new TestController(log);
        var executing = new ActionExecutingContext(action, [], new Dictionary<string, object?>(), controller);
        var error = new InvalidOperationException("Test action failed");
        var filter = new OqtLogWebApiAttribute(historyGroup);

        var failure = await Record.ExceptionAsync(() => filter.OnActionExecutionAsync(executing, () =>
            actionThrows
                ? Task.FromException<ActionExecutedContext>(error)
                : Task.FromResult(new ActionExecutedContext(action, [], controller))));

        Same(actionThrows ? error : null, failure);
        Equal((historyGroup, (ILog)log), Single(store.Entries));
        var timer = Single(log.Entries, entry => entry.WrapOpen);
        True(timer.WrapOpenWasClosed);
    }

    private sealed class TestController(ILog log) : ControllerBase, IHasLog
    {
        public ILog Log { get; } = log;
    }

    private sealed class FakeLogStore : ILogStore
    {
        public List<(string Group, ILog Log)> Entries { get; } = [];

        public LogStoreEntry? Add(string segment, ILog log)
        {
            Entries.Add((segment, log));
            return null;
        }

        public LogStoreEntry? ForceAdd(string key, ILog log) => throw new NotSupportedException();
    }
}
