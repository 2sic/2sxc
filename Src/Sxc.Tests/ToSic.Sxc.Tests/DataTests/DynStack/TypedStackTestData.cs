using System.Collections.Generic;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DataTests.DynStack
{
    internal class TypedStackTestData
    {
        public static IEnumerable<object[]> KeysAndExpectations => new[]
        {
            new object[] {"Key1", true },
            new object[] {"dummy", false },
            new object[] {"Part1", true },
            new object[] {"Deep1", true },
            new object[] {"Deep1.Sub1", true },
            new object[] {"Deep", true },
            new object[] {"Deep.Deeper", true },
            new object[] {"Deep.NotDeeper", false },
            new object[] {"Deep.NotDeeper.ReallyNot", false },
            new object[] {"Deep.Deeper.Value", true },
            new object[] {"Data.Deeper.NotValue", false },
        };

        public static object Anon1 = new
        {
            Key1 = "hello",
            Key2 = "goodbye",
            Deep1 = new
            {
                Sub1 = "hello",
            }
        };

        public static object Anon2 = new
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
        };

        public static ITypedStack GetStackForKeysUsingAnon(DynAndTypedTestsBase parent)
        {
            var part1 = parent.Obj2Typed(Anon1);
            var part2 = parent.Obj2Typed(Anon2);
            var stack = parent.Factory.AsStack(new[] { part1, part2 });
            return stack;
        }
        public static ITypedStack GetStackForKeysUsingJson(DynAndTypedTestsBase parent)
        {
            var part1 = parent.Obj2Json2Typed(Anon1);
            var part2 = parent.Obj2Json2Typed(Anon2);
            var stack = parent.Factory.AsStack(new[] { part1, part2 });
            return stack;
        }

    }
}
