using System.Reflection;
using ToSic.Eav.Sys.Insights;
using Microsoft.Extensions.Logging;
using static Xunit.Assert;

namespace ToSic.Sxc.WebApi.Tests.Insights;

public class InsightsLogRendererTests
{
    [Theory]
    [InlineData("ToSic.AppLoad", "AppLoad")]
    [InlineData("ToSic.GlobalTypes", "GlobalTypes")]
    public void DumpTree_RendersMatchingMelEvents_ForAffectedConsumer(string category, string consumer)
    {
        var store = new InsightsLogStore();
        var provider = new InsightsLoggerProvider(store);
        using var factory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddProvider(provider);
        });
        var log = new MelLogFactory(factory).Create(category["ToSic.".Length..], null, new());
        var unrelated = new MelLogFactory(factory).Create("Unrelated", null, new());

        log.A($"message-{consumer}", cPath: $"source-{consumer}");
        log.Ex(new InvalidOperationException($"exception-{consumer}"));
        log.Fn<string>(message: "completion")!.Return($"result-{consumer}");
        unrelated.A("unrelated-message");

        var html = DumpTree(log, new MelInsightsLogSnapshotReader(store).Snapshot());

        Contains($"message-{consumer}", html);
        Contains($"result-{consumer}", html);
        Contains($"source-{consumer}", html);
        Contains($"exception-{consumer}", html);
        DoesNotContain("unrelated-message", html);
        Contains("retained events", html);
    }

    [Fact]
    public void DumpTree_RendersLegacyLog()
    {
        var log = new Log("Legacy", message: "legacy-message");

        var html = DumpTree(log, new(false, []));

        Contains("legacy-message", html);
    }

    private static string DumpTree(ILog log, InsightsLogSnapshot snapshot)
    {
        // Exercise the internal EAV renderer without widening its production API.
        var assembly = typeof(InsightsProvider).Assembly;
        var type = assembly.GetType("ToSic.Eav.Sys.Insights.Logs.InsightsLogsHelper", throwOnError: true)!;
        var helper = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, binder: null, [snapshot], culture: null)!;
        var method = type.GetMethod("DumpTree", BindingFlags.Instance | BindingFlags.NonPublic, binder: null, [typeof(string), typeof(ILog)], modifiers: null)!;

        return (string)method.Invoke(helper, ["test log", log])!;
    }

}
