using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ToSic.Sxc.Tests.Data.DynamicJacket;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketValues : DynamicJacketTestBase
    {
        [TestMethod]
        public void ObjectWithBoolProperty()
        {
            var (dyn, _, _) = PrepareTest(new
            {
                TrueBoolType = true,
                FalseBoolType = false
            });

            IsTrue(dyn.TrueBoolType);
            IsFalse(dyn.FalseBoolType);
            IsNull(dyn.something);
        }

        [TestMethod]
        public void ObjectWithNumberProperty()
        {
            var (dyn, _, original) = PrepareTest(new 
            {
                IntType = 32,
                BigIntType = 9007199254740991,
                DoubleType = 0.333333333333333314829616256247390992939472198486328125,
            });
            AreEqual<int>(original.IntType, dyn.IntType);
            AreEqual<long>(original.BigIntType, dyn.BigIntType);
            AreEqual<double>(original.DoubleType,dyn.DoubleType);
        }

        [TestMethod]
        public void ObjectWithStringProperty()
        {
            var jsonString = @"{ 
                ""StringType"": ""stringValue"", 
                ""GuidType"": ""00000000-0000-0000-0000-000000000000"",
            }";
            AreEqual<string>("stringValue", AsDynamic(jsonString).StringType);
            AreEqual<string>("00000000-0000-0000-0000-000000000000", AsDynamic(jsonString).GuidType);
        }

        [TestMethod]
        public void ObjectWithDateTimeProperty()
        {
            var jsonString = @"{ 
                ""DateTimeType"": ""2022-11-09T23:00:00.000"", 
                ""ZuluTimeType"": ""2021-04-15T21:01:00.000Z"", 
            }";
            AreEqual<DateTime>(new DateTime(2022,11,9, 23,0,0), AsDynamic(jsonString).DateTimeType);
            AreEqual<DateTime>(new DateTime(2021, 4, 15, 21, 1, 0, DateTimeKind.Utc), AsDynamic(jsonString).ZuluTimeType);
        }

        [TestMethod]
        public void ObjectWithNullProperty()
        {
            var jsonString = @"{ 
                ""NullType"": null, 
                ""UndefinedType"": undefined, 
            }";
            IsNull(AsDynamic(jsonString).NullType);
            IsNull(AsDynamic(jsonString).UndefinedType);
            IsNull(AsDynamic(jsonString).NonExistingProperty);
        }
    }
}