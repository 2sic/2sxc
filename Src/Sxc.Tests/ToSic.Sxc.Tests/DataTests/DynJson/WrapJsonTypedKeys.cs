using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class WrapJsonTypedKeys : DynAndTypedTestsBase
    {
        private bool HasKey(ITyped typed, string key)
        {
            return typed.ContainsKey(key);
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