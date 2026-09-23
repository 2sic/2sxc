using System.Reflection;
using System.Globalization;
using System.Xml.Linq;
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
        var log = new MelLogFactory(factory).Create(category.Substring("ToSic.".Length), null, new());
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

    [Theory]
    [InlineData(1700L, "1.7s")]
    [InlineData(45L, "45ms")]
    [InlineData(0L, "0ms")]
    [InlineData(null, "")]
    public void LogHistoryList_FormatsLongestRetainedDuration(long? milliseconds, string expected)
    {
        var snapshot = TimingSnapshot(milliseconds / 2, milliseconds, milliseconds / 3);

        var html = Render(snapshot, "LogHistoryList", typeof(string), "");
        var cell = XElement.Parse("<root>" + html + "</root>")
            .Descendants("tbody").Single().Element("tr")!.Elements("td").Last();

        expected = expected.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        Equal(expected.Length == 0 ? "" : "⌚ " + expected, cell.Value.Trim());
        Equal("Longest recorded operation; not total request duration", (string?)cell.Attribute("title"));
        if (milliseconds != null)
        {
            var badge = cell.Element("span")!;
            Equal("time", (string?)badge.Attribute("class"));
            Equal("background: #b9b8ff", (string?)badge.Attribute("style"));
        }
    }

    [Fact]
    public void DumpTree_FormatsOlderMelDuration_WithRelativeTime()
    {
        var snapshot = TimingSnapshot(1700);

        var html = Render(snapshot, "DumpTree", typeof(InsightsLogGroupSnapshot), Single(snapshot.Groups));
        var line = XElement.Parse("<root>" + html + "</root>").Descendants("span")
            .Single(element => (string?)element.Attribute("class") == "log-line");
        var timing = Single(line.Elements("span"), element => (string?)element.Attribute("class") == "time");

        DoesNotContain("12:00:00.000", line.Value);
        Equal($"⌚ {1.7:0.#}s | {0:F}", timing.Value.Trim());
        Equal("background: #b9b8ff", (string?)timing.Attribute("style"));
    }

    [Fact]
    public void DumpTree_OrdersMelCallsByStartAndKeepsSubMillisecondTiming()
    {
        var start = new DateTime(2026, 9, 23, 12, 0, 0, DateTimeKind.Utc);
        var store = new InsightsLogStore();
        void Add(long sequence, string message, int completedMs, int? startedMs = null, long? durationTicks = null, long? durationMs = null)
            => store.Append(new()
            {
                Sequence = sequence,
                TimestampUtc = start.AddMilliseconds(completedMs),
                StartedUtc = startedMs is { } ms ? start.AddMilliseconds(ms) : null,
                Category = "ToSic.Module",
                Segment = "module",
                TraceId = "request",
                Message = message,
                DurationTicks = durationTicks,
                DurationMilliseconds = durationMs
            });
        // Completion order differs from start order; tiny durations must survive both ordering and formatting.
        Add(1, "plain", 250);
        Add(2, "untimed", 400, 120);
        Add(3, "tiny", 600, 150, 1300, 0);
        Add(4, "microscopic", 700, 180, 1, 0);
        Add(5, "root", 3000, 0, TimeSpan.FromSeconds(3).Ticks, 3000);
        var snapshot = new MelInsightsLogSnapshotReader(store).Snapshot();

        var html = Render(snapshot, "DumpTree", typeof(InsightsLogGroupSnapshot), Single(snapshot.Groups));
        var lines = XElement.Parse("<root>" + html + "</root>").Descendants("span")
            .Where(element => (string?)element.Attribute("class") == "log-line").ToArray();
        string Timing(int index) => Single(lines[index].Elements("span"), element => (string?)element.Attribute("class") == "time").Value.Trim();

        Equal(start, Single(snapshot.Groups).TimestampUtc);
        Contains("root", lines[0].Value);
        Contains("untimed", lines[1].Value);
        Contains("tiny", lines[2].Value);
        Contains("microscopic", lines[3].Value);
        Contains("plain", lines[4].Value);
        Equal($"⌚ 3s | {0:F}", Timing(0));
        Equal($"{0.12:F} since ▶️", Timing(1));
        Equal($"⌚ {0.13:0.####}ms | {0.15:F}", Timing(2));
        Equal($"⌚ {0.0001:0.####}ms | {0.18:F}", Timing(3));
        Equal($"{0.25:F} since ▶️", Timing(4));
    }

    [Fact]
    public void ShowTime_PreservesLegacyTimingBarAndPercentages()
    {
        var log = new Log("Legacy", message: "timed");
        var entry = Single(log.Entries);
        entry.Elapsed = TimeSpan.FromMilliseconds(1700);
        var type = typeof(InsightsProvider).Assembly.GetType("ToSic.Eav.Sys.Insights.HtmlHelpers.InsightsTime", throwOnError: true)!;
        var helper = Activator.CreateInstance(type, [entry.Elapsed])!;

        var html = (string)type.GetMethod("ShowTime", [typeof(Entry), typeof(TimeSpan), typeof(DateTime)])!
            .Invoke(helper, [entry, TimeSpan.FromMilliseconds(3400), entry.Created])!;
        var badge = XElement.Parse(html);

        Equal($"⌚ {1.7:0.#}s 50% | 100% | {0:F}", badge.Value.Trim());
        Equal("background: linear-gradient(90deg, #bbbbbb 0%, #b9b8ff 0%, #b9b8ff 100%, #eeeeee 100%)", (string?)badge.Attribute("style"));
    }

    [Fact]
    public void DumpTree_LegacySnapshotShowsRelativeTimeAndPreciseDuration()
    {
        var store = new LogStoreLive();
        var segment = "renderer-" + Guid.NewGuid();
        var log = new Log("Legacy");
        var call = log.Fn(timer: true)!;
        log.A("inside");
        call.Done();
        call.Entry!.Elapsed = TimeSpan.FromTicks(1300);
        store.ForceAdd(segment, log);
        try
        {
            var group = Single(new LegacyInsightsLogSnapshotReader(store).ListGroups(), item => item.Segments.Contains(segment));

            var html = Render(new(false, [group]), "DumpTree", typeof(InsightsLogGroupSnapshot), group);
            var lines = XElement.Parse("<root>" + html + "</root>").Descendants("span")
                .Where(element => (string?)element.Attribute("class") == "log-line").ToArray();
            string Timing(int index) => Single(lines[index].Elements("span"), element => (string?)element.Attribute("class") == "time").Value.Trim();

            Equal(2, lines.Length);
            StartsWith($"⌚ {0.13:0.####}ms | ", Timing(0));
            EndsWith(" since ▶️", Timing(1));
            DoesNotContain(":", Timing(0));
        }
        finally
        {
            store.FlushSegment(segment);
        }
    }

    private static InsightsLogSnapshot TimingSnapshot(params long?[] durations)
    {
        var store = new InsightsLogStore();
        for (var index = 0; index < durations.Length; index++)
            store.Append(new()
            {
                Sequence = index + 1,
                TimestampUtc = new DateTime(2026, 9, 23, 12, 0, 0, DateTimeKind.Utc).AddSeconds(index),
                Category = "ToSic.Module",
                Segment = "module",
                TraceId = "request",
                Message = "operation",
                DurationMilliseconds = durations[index]
            });
        return new MelInsightsLogSnapshotReader(store).Snapshot();
    }

    private static string DumpTree(ILog log, InsightsLogSnapshot snapshot)
        => Render(snapshot, "DumpTree", typeof(ILog), log);

    private static string Render(InsightsLogSnapshot snapshot, string methodName, Type argumentType, object argument)
    {
        // Exercise the internal EAV renderer without widening its production API.
        var assembly = typeof(InsightsProvider).Assembly;
        var type = assembly.GetType("ToSic.Eav.Sys.Insights.Logs.InsightsLogsHelper", throwOnError: true)!;
        // The helper's constructor is public even though the type is internal.
        var helper = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, binder: null, [snapshot], culture: null)!;
        var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, binder: null, [typeof(string), argumentType], modifiers: null)!;

        return (string)method.Invoke(helper, ["module", argument])!;
    }

}
