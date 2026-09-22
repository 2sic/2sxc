using System.Collections.Immutable;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sys.Logging;

namespace ToSic.Sxc.Dnn;

public class DnnMelDiagnosticTests
{
    [Fact]
    public void CurrentTrace_OnlyReadsAndFormatsSelectedTrace()
    {
        var store = new InsightsLogStore();
        store.Append(new()
        {
            Sequence = 1,
            Category = "ToSic.Selected",
            Message = "selected",
            Result = "result",
            DurationMilliseconds = 1500,
            TraceId = "trace-one",
            Exception = new("TestException", "custom message", null, "full detached exception details", ImmutableDictionary<string, string?>.Empty)
        });
        store.Append(new() { Sequence = 2, Category = "ToSic.Other", Message = "other", TraceId = "trace-two" });
        var reader = new MelInsightsLogSnapshotReader(store);

        var events = DnnLogging.CurrentTraceEvents(reader, "trace-one").ToArray();
        var dump = DnnLogging.DumpCurrentTrace(reader, "trace-one");

        Equal("selected", Single(events).Message);
        Contains("ToSic.Selected - selected", dump);
        Contains("=>result", dump);
        Contains($"⌚ {TimeSpan.FromMilliseconds(1500).TotalSeconds}s", dump);
        Contains("custom message", dump);
        Contains("full detached exception details", dump);
        DoesNotContain("other", dump);
        Empty(DnnLogging.CurrentTraceEvents(reader, null));
        Empty(DnnLogging.CurrentTraceEvents(reader, "trace-missing"));
    }
}
