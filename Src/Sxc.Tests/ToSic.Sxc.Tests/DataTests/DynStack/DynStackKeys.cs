using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynStack
{
    [TestClass]
    public class DynStackKeys: DynStackTestBase
    {

        [TestMethod]
        [DataRow("Key1", true)]
        [DataRow("dummy", false)]
        [DataRow("Part1", true)]
        [DataRow("Deep1", true)]
        [DataRow("Deep1.Sub1", true)]
        [DataRow("Deep", true)]
        [DataRow("Deep.Deeper", true)]
        [DataRow("Deep.NotDeeper", false)]
        [DataRow("Deep.NotDeeper.ReallyNot", false)]
        [DataRow("Deep.Deeper.Value", true)]
        [DataRow("Data.Deeper.NotValue", false)]
        [ExpectedException(typeof(NotImplementedException))]
        public void Keys(string key, bool expected) => AreEqual(expected, StackForKeys.ContainsKey(key));


        private ITypedStack StackForKeys => _stackForKeys ?? (_stackForKeys ?? GetStackForKeys());
        private ITypedStack _stackForKeys;

        private ITypedStack GetStackForKeys()
        {
            var anon = new
            {
                Key1 = "hello",
                Key2 = "goodbye",
                Deep1 = new
                {
                    Sub1 = "hello",
                }
            };
            var typed = TypedFromObject(anon);
            var part2 = TypedFromObject(new
            {
                Key1 = "hello 2",
                Part1 = "test",
                Part2 = "test",
                Deep = new
                {
                    Deeper = new
                    {
                        Value = "hello",
                    },
                },
            });
            var stack = Factory.AsStack(new [] { typed, part2 });
            return stack;
        }
    }
}
