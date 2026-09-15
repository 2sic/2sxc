using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Services.Sys.CodeApiServiceHelpers;
using ToSic.Sys.Run.Startup;
using ToSic.Sys.Users;
using static Xunit.Assert;

namespace Tests.ToSic.ToSxc.WebApi.WebApi.Sys;

[CollectionDefinition(nameof(CodeApiServiceLoggingTests), DisableParallelization = true)]
public sealed class CodeApiServiceLoggingTestCollection;

[Collection(nameof(CodeApiServiceLoggingTests))]
public class CodeApiServiceLoggingTests
{
    [Fact]
    public void ReusedService_AdmitsEachStandaloneUse_ButNotRequestUses()
    {
        using var ctx = new TestContext();

        ctx.Service.Use("standalone before");
        var request = ctx.Store.Add("request", new Log("Tst.Request"))!;
        using (ctx.Logger.BeginExecution(request, ctx.Source, "request"))
            ctx.Service.Use("inside request");
        ctx.Service.Use("standalone after");

        Equal(2, ctx.Store.Snapshot("code-api-service").Count);
        Equal(new[] { "standalone before", "standalone after" },
            ctx.Store.Snapshot("code-api-service").SelectMany(snapshot => snapshot.Entries).Select(entry => entry.Message));
        Equal("inside request", Single(Single(ctx.Store.Snapshot("request")).Entries).Message);
    }

    [Fact]
    public async Task ReusedService_ParallelRequestAndStandaloneUse_DoNotCrossContaminate()
    {
        using var ctx = new TestContext();
        var request = ctx.Store.Add("request", new Log("Tst.Request"))!;
        using var start = new Barrier(2);

        var requestUse = Task.Run(() =>
        {
            using var execution = ctx.Logger.BeginExecution(request, ctx.Source, "request");
            start.SignalAndWait();
            ctx.Service.Use("inside request");
        });
        var standaloneUse = Task.Run(() =>
        {
            start.SignalAndWait();
            ctx.Service.Use("standalone");
        });
        await Task.WhenAll(requestUse, standaloneUse);

        Equal("inside request", Single(Single(ctx.Store.Snapshot("request")).Entries).Message);
        Equal("standalone", Single(Single(ctx.Store.Snapshot("code-api-service")).Entries).Message);
    }

    private sealed class TestContext : IDisposable
    {
        private readonly ServiceProvider _services;
        internal readonly ILogStoreLive Store;
        internal readonly ILogger Logger;
        internal readonly ActivitySource Source = new("Test.CodeApiService");
        internal readonly TestService Service;

        internal TestContext()
        {
            _services = new ServiceCollection().AddSysCoreLogging().BuildServiceProvider();
            Store = _services.GetRequiredService<ILogStoreLive>();
            var factory = _services.GetRequiredService<ILoggerFactory>();
            Logger = factory.CreateLogger("Test.CodeApiService");
            LogEventBridge.SetSink(new MicrosoftLoggerEventSink(factory, false));
            Service = new(new(
                _services,
                new LazySvc<ILogStore>(_services),
                new LazySvc<IUser>(_services),
                new LazySvc<ISite>(_services),
                new LazySvc<IZoneMapper>(_services),
                new LazySvc<IAppsCatalog>(_services)));
        }

        public void Dispose()
        {
            LogEventBridge.SetSink(null);
            Source.Dispose();
            _services.Dispose();
        }
    }

    private sealed class TestService(CodeApiServiceBase.Dependencies services)
        : CodeApiServiceBase(services, "Tst.CodeApi")
    {
        internal void Use(string message)
        {
            MakeSureLogIsInHistory();
            Log.A(message);
        }
    }
}
