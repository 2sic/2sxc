using ToSic.Lib.Logging;

namespace ToSic.Eav.Core.Tests.LogTests;

[TestClass]
public class LogFnTests : LogTestBase
{
    [TestMethod]
    public void NoReturnBasic()
    {
        var log = L("Test");
        var call = log.Fn();
        AreEqual(1, log.Entries.Count);
        call.Done("ok");
            
        AreEqual(2, log.Entries.Count); // Another for results
        var resultEntry = log.Entries.First();
        AreEqual("ok", resultEntry.Result);
    }

    [TestMethod]
    public void NoReturnAll()
    {
        var log = L("Test");
        var call = log.Fn($"something: {7}", "start msg", true);
        IsTrue(call.Timer.ElapsedMilliseconds < 1);
        AreEqual(1, log.Entries.Count);
        System.Threading.Thread.Sleep(10); // wait 10 ms
        call.Done("ok");

        IsTrue(call.Timer.ElapsedMilliseconds > 9);
            
        AreEqual(2, log.Entries.Count); // Another for results
        var resultEntry = log.Entries.First();
        AreEqual("ok", resultEntry.Result);
    }

    [TestMethod]
    public void GenericBasic()
    {
        var log = L("Test");
        var call = log.Fn<string>();
            
        AreEqual(1, log.Entries.Count);  // Should have one when starting
        var result = call.Return("result", "ok");
        AreEqual("result", result);

        AreEqual(2, log.Entries.Count);  // Another for results
        var resultEntry = log.Entries.First();
        AreEqual("ok", resultEntry.Result);
    }
}