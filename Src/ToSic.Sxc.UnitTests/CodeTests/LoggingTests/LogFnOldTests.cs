//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Linq;

//namespace ToSic.Eav.Core.Tests.LogTests
//{
//    
//    public class LogFnOldTests : LogTestBase
//    {
//        [Fact]
//        public void NoReturnBasic()
//        {
//            var log = SL("Test");
//            var call = log.Fn();
//            Assert.Equal(1, log.Entries.Count);
//            call.Done("ok");
            
//            Assert.Equal(2, log.Entries.Count); // Another for results
//            var resultEntry = log.Entries.First();
//            Assert.Equal("ok", resultEntry.Result);
//        }

//        [Fact]
//        public void NoReturnAll()
//        {
//            var log = SL("Test");
//            var call = log.Fn($"something: {7}", "start msg", true);
//            Assert.IsTrue(call.Stopwatch.ElapsedMilliseconds < 1);
//            Assert.Equal(1, log.Entries.Count);
//            System.Threading.Thread.Sleep(10); // wait 10 ms
//            call.Done("ok");

//            Assert.IsTrue(call.Stopwatch.ElapsedMilliseconds > 9);
            
//            Assert.Equal(2, log.Entries.Count); // Another for results
//            var resultEntry = log.Entries.First();
//            Assert.Equal("ok", resultEntry.Result);
//        }

//        [Fact]
//        public void GenericBasic()
//        {
//            var log = SL("Test");
//            var call = log.Fn<string>();
            
//            Assert.Equal(1, log.Entries.Count);  // Should have one when starting
//            var result = call.Return("result", "ok");
//            Assert.Equal("result", result);

//            Assert.Equal(2, log.Entries.Count);  // Another for results
//            var resultEntry = log.Entries.First();
//            Assert.Equal("ok", resultEntry.Result);
//        }
//    }
//}
