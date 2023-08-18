using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class DynJsonValues : DynAndTypedTestsBase
    {
        private bool HasKey(ITyped typed, string key)
        {
            return typed.ContainsKey(key);
        }

        [TestMethod]
        public void JsonBoolProperty()
        {
            var (dyn, _, _) = DynJsonAndOriginal(new
            {
                TrueBoolType = true,
                FalseBoolType = false
            });

            IsTrue(dyn.TrueBoolType);
            IsFalse(dyn.FalseBoolType);
            IsNull(dyn.something);
        }
        [TestMethod]
        public void JsonBoolPropertyKeys_Typed()
        {
            var typed = Obj2Json2Typed(new
            {
                TrueBoolType = true,
                FalseBoolType = false
            });

            IsTrue(typed.Bool("TrueBoolType"));
            IsFalse(typed.Bool("FalseBoolType"));
            IsFalse(typed.Bool("something"));

            IsTrue(HasKey(typed, "TrueBoolType"));
            IsTrue(HasKey(typed, "TrueBoolTYPE"));
            IsTrue(HasKey(typed, "FalseBoolType"));
            IsFalse(HasKey(typed, "something"));
        }

        [TestMethod]
        public void JsonNumberProperty()
        {
            var (dyn, _, original) = DynJsonAndOriginal(new 
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
        public void ExpectCountOfPropertiesOnNonArray()
        {
            var test = DynJsonAndOriginal(new { Name = "Test" });
            AreEqual(1, test.Dyn.Count);

            var test2 = DynJsonAndOriginal(new { Name = "Test", Age = 3, Birthday = new DateTime(2022, 1, 1) });
            AreEqual(3, test2.Dyn.Count);
        }

        [TestMethod]
        public void EnumerateProperties()
        {
            var test = DynJsonAndOriginal(new { Name = "Test" });
            var testList = (test.Dyn as IEnumerable<object>).ToList();
            AreEqual(1, testList.Count);
            AreEqual("Name", testList.First().ToString());

            var test2 = DynJsonAndOriginal(new { Name = "Test", Age = 3, Birthday = new DateTime(2022, 1, 1) });
            var testList2 = (test2.Dyn as IEnumerable<object>).ToList();
            AreEqual(3, testList2.Count);
            IsTrue(testList2.Contains("Name"));
            IsTrue(testList2.Contains("Birthday"));
        }


        [TestMethod]
        public void ObjectWithStringProperty()
        {
            var jsonString = @"{ 
                ""StringType"": ""stringValue"", 
                ""GuidType"": ""00000000-0000-0000-0000-000000000000"",
            }";
            AreEqual<string>("stringValue", Json2Dyn(jsonString).StringType);
            AreEqual<string>("00000000-0000-0000-0000-000000000000", Json2Dyn(jsonString).GuidType);
        }

        [TestMethod]
        public void ObjectWithDateTimeProperty()
        {
            var jsonString = @"{ 
                ""DateTimeType"": ""2022-11-09T23:00:00.000"", 
                ""ZuluTimeType"": ""2021-04-15T21:01:00.000Z"", 
            }";
            AreEqual<DateTime>(new DateTime(2022,11,9, 23,0,0), Json2Dyn(jsonString).DateTimeType);
            AreEqual<DateTime>(new DateTime(2021, 4, 15, 21, 1, 0, DateTimeKind.Utc), Json2Dyn(jsonString).ZuluTimeType);
        }

        [TestMethod]
        public void ObjectWithNullProperty()
        {
            var dynJson = Json2Dyn(@"{ 
                ""NullType"": null, 
                ""UndefinedType"": null, // undefined would not be valid
            }");
            IsNull(dynJson.NullType);
            IsNull(dynJson.UndefinedType);
            IsNull(dynJson.NonExistingProperty);
        }

        [TestMethod]
        public void Keys()
        {
            var anon = new
            {
                Key1 = "hello",
                Key2 = "goodbye"
            };
            var typed = Obj2Json2Typed(anon);
            IsTrue(typed.ContainsKey("Key1"));
            IsFalse(typed.ContainsKey("Nonexisting"));
            IsTrue(typed.Keys().Any());
            AreEqual(2, typed.Keys().Count());
            AreEqual(1, typed.Keys(only: new[] { "Key1" }).Count());
            AreEqual(0, typed.Keys(only: new[] { "Nonexisting" }).Count());
        }

    }
}