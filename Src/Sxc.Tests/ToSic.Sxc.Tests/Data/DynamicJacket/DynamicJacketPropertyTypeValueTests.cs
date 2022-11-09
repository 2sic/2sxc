using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketPropertyTypeValueTests
    {
        public dynamic AsDynamic(string jsonString) => DynamicJacket.AsDynamicJacket(jsonString);

        [TestMethod]
        public void ObjectWithBoolProperty()
        {
            var jsonString = @"{ 
                ""TrueBoolType"": true, 
                ""FalseBoolType"": false, 
            }";
            Assert.IsTrue(AsDynamic(jsonString).TrueBoolType);
            Assert.IsFalse(AsDynamic(jsonString).FalseBoolType);
        }

        [TestMethod]
        public void ObjectWithNumberProperty()
        {
            var jsonString = @"{ 
                ""IntType"": 32, 
                ""BigIntType"": 9007199254740991, 
                ""DoubleType"": 0.333333333333333314829616256247390992939472198486328125, 
            }";
            Assert.AreEqual<int>(32, AsDynamic(jsonString).IntType);
            Assert.AreEqual<long>(9007199254740991, AsDynamic(jsonString).BigIntType);
            Assert.AreEqual<double>(0.333333333333333314829616256247390992939472198486328125,AsDynamic(jsonString).DoubleType);
        }

        [TestMethod]
        public void ObjectWithStringProperty()
        {
            var jsonString = @"{ 
                ""StringType"": ""stringValue"", 
                ""GuidType"": ""00000000-0000-0000-0000-000000000000"",
            }";
            Assert.AreEqual<string>("stringValue", AsDynamic(jsonString).StringType);
            Assert.AreEqual<string>("00000000-0000-0000-0000-000000000000", AsDynamic(jsonString).GuidType);
        }

        [TestMethod]
        public void ObjectWithDateTimeProperty()
        {
            var jsonString = @"{ 
                ""DateTimeType"": ""2022-11-09T23:00:00.000"", 
                ""ZuluTimeType"": ""2021-04-15T21:01:00.000Z"", 
            }";
            Assert.AreEqual<DateTime>(new DateTime(2022,11,9, 23,0,0), AsDynamic(jsonString).DateTimeType);
            Assert.AreEqual<DateTime>(new DateTime(2021, 4, 15, 21, 1, 0, DateTimeKind.Utc), AsDynamic(jsonString).ZuluTimeType);
        }

        [TestMethod]
        public void ObjectWithNullProperty()
        {
            var jsonString = @"{ 
                ""NullType"": null, 
                ""UndefinedType"": undefined, 
            }";
            Assert.IsNull(AsDynamic(jsonString).NullType);
            Assert.IsNull(AsDynamic(jsonString).UndefinedType);
            Assert.IsNull(AsDynamic(jsonString).NonExistingProperty);
        }
    }
}