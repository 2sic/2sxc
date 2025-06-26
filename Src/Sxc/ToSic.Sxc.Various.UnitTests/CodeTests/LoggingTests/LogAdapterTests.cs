//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Linq;
//using ToSic.Lib.Logging;


//namespace ToSic.Eav.Core.Tests.LogTests
//{
//    
//    public class LogAdapterTests : LogTestBase
//    {

        
//        [Fact]
//        public void NoReturnBasic()
//        {
//            var log = LA("Test");

//            var call = log.Fn();
//            Assert.Equal(1, log.L.Entries.Count);

//            call.Done("ok");
//            Assert.Equal(2, log.L.Entries.Count); // Another for results

//            var resultEntry = log.L.Entries.First();
//            Assert.Equal("ok", resultEntry.Result);
//        }

//        [Fact]
//        public void NoReturnAll()
//        {
//            var log = LA("Test");

//            var call = log.Fn($"something: {7}", "start msg", true);
//            Assert.IsTrue(call.Stopwatch.ElapsedMilliseconds < 1);
//            Assert.Equal(1, log.L.Entries.Count);
//            System.Threading.Thread.Sleep(10); // wait 10 ms

//            call.Done("ok");
//            Assert.IsTrue(call.Stopwatch.ElapsedMilliseconds > 9);
//            Assert.Equal(2, log.L.Entries.Count); // Another for results

//            var resultEntry = log.L.Entries.First();
//            Assert.Equal("ok", resultEntry.Result);
//        }

//        [Fact]
//        public void GenericBasic()
//        {
//            var log = LA("Test");

//            var call = log.Fn<string>();
//            Assert.Equal(1, log.L.Entries.Count);  // Should have one when starting
            
//            var result = call.Return("result", "ok");
//            Assert.Equal("result", result);
//            Assert.Equal(2, log.L.Entries.Count);  // Another for results

//            var resultEntry = log.L.Entries.First();
//            Assert.Equal("ok", resultEntry.Result);
//        }
//    }
//}
