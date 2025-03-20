using ToSic.Lib.Logging;

namespace ToSic.Eav.Core.Tests.LogTests;


public class LogFnTests : LogTestBase
{
    [Fact]
    public void NoReturnBasic()
    {
        var log = L("Test");
        var call = log.Fn();
        Equal(1, log.Entries.Count);
        call.Done("ok");
            
        Equal(2, log.Entries.Count); // Another for results
        var resultEntry = log.Entries.First();
        Equal("ok", resultEntry.Result);
    }

    [Fact]
    public void NoReturnAll()
    {
        var log = L("Test");
        var call = log.Fn($"something: {7}", "start msg", true);
        True(call.Timer.ElapsedMilliseconds < 1);
        Equal(1, log.Entries.Count);
        System.Threading.Thread.Sleep(10); // wait 10 ms
        call.Done("ok");

        True(call.Timer.ElapsedMilliseconds > 9);
            
        Equal(2, log.Entries.Count); // Another for results
        var resultEntry = log.Entries.First();
        Equal("ok", resultEntry.Result);
    }

    [Fact]
    public void GenericBasic()
    {
        var log = L("Test");
        var call = log.Fn<string>();
            
        Equal(1, log.Entries.Count);  // Should have one when starting
        var result = call.Return("result", "ok");
        Equal("result", result);

        Equal(2, log.Entries.Count);  // Another for results
        var resultEntry = log.Entries.First();
        Equal("ok", resultEntry.Result);
    }
}