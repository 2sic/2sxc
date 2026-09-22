using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ToSic.Sxc.Dnn;

public class DnnRequestLogCorrelationTests
{
    [Fact]
    public void Ensure_SharesOneOwnedActivityAndClearsItOnCompletion()
    {
        var items = new Hashtable();
        Action<IDictionary>? completed = null;
        var registrations = 0;

        var first = DnnRequestLogCorrelation.Ensure(items, callback =>
        {
            registrations++;
            completed = callback;
        });
        var second = DnnRequestLogCorrelation.Ensure(items, callback =>
        {
            registrations++;
            completed = callback;
        });

        Same(first, second);
        Same(first, Activity.Current);
        Equal("ToSic.Dnn.Request", first.OperationName);
        Equal(ActivityIdFormat.W3C, first.IdFormat);
        Equal(1, registrations);
        NotNull(completed);
        completed(items);
        Null(Activity.Current);
    }

    [Fact]
    public void Ensure_KeepsExistingActivity()
    {
        var items = new Hashtable();
        var registrations = 0;
        using var external = new Activity("external").SetIdFormat(ActivityIdFormat.W3C).Start();

        var owned = DnnRequestLogCorrelation.Ensure(items, _ => registrations++);

        Null(owned);
        Same(external, Activity.Current);
        Equal(0, registrations);
    }

    [Fact]
    public void Ensure_SeparatesParallelRequests()
    {
        var traceIds = new ConcurrentBag<string>();
        var cleared = new ConcurrentBag<bool>();

        Parallel.Invoke(
            () => RunRequest(traceIds, cleared),
            () => RunRequest(traceIds, cleared));

        Equal(2, traceIds.Distinct().Count());
        All(cleared, True);
    }

    [Fact]
    public void Complete_StopsOwnedActivityInFinallyAfterException()
    {
        var items = new Hashtable();
        Action<IDictionary>? completed = null;
        DnnRequestLogCorrelation.Ensure(items, callback => completed = callback);

        try
        {
            throw new InvalidOperationException();
        }
        catch (InvalidOperationException)
        {
            // Simulate an exceptional request path.
        }
        finally
        {
            completed!(items);
        }

        Null(Activity.Current);
    }

    private static void RunRequest(ConcurrentBag<string> traceIds, ConcurrentBag<bool> cleared)
    {
        var items = new Hashtable();
        Action<IDictionary>? completed = null;
        var activity = DnnRequestLogCorrelation.Ensure(items, callback => completed = callback);
        traceIds.Add(activity!.TraceId.ToString());
        completed!(items);
        cleared.Add(Activity.Current == null);
    }
}
