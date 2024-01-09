using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Internal.Dynamic;
using ToSic.Sxc.Data.Internal.Wrapper;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapDicBasic: TestBaseSxcDb
    {
        private WrapDictionaryDynamic<TKey, TValue> ToDyn<TKey, TValue>(Dictionary<TKey, TValue> dic)
            => GetService<CodeDataWrapper>().FromDictionary(dic);

        [TestMethod]
        public void BasicUseDictionary()
        {
            var dic = new Dictionary<string, object>
            {
                ["Name"] = "2sxc",
                ["Description"] = "",
                ["Founded"] = 2012,
                ["Birthday"] = new DateTime(2012, 5, 4),
                ["Truthy"] = true,
            };

            var typed = ToDyn(dic); // as ITyped;
            dynamic dynAnon = typed;

            IsNull(dynAnon.NotExisting);
            AreEqual(dic["Name"], dynAnon.Name);
            AreEqual(dic["Name"], dynAnon.naME, "Should be the same irrelevant of case");
            AreEqual(dic["Birthday"], dynAnon.Birthday, "dates should be the same");
            AreEqual(dic["Truthy"], dynAnon.truthy);

            // 2023-08-07 2dm - DynamicFromDictionary does not implement ITyped as of now

            //IsTrue(typed.Has("Name"));
            //IsTrue(typed.Has("NAME"));
            //IsTrue(typed.Has("Description"));
            //IsFalse(typed.Has("NonexistingField"));
        }
        [TestMethod]
        public void Keys()
        {
            var anon = new Dictionary<string, object>
            {
                ["Key1"] = "hello",
                ["Key2"] = "goodbye"
            };
            var typed = ToDyn(anon) as IHasKeys;
            IsTrue(typed.ContainsKey("Key1"));
            IsFalse(typed.ContainsKey("Nonexisting"));
            IsTrue(typed.Keys().Any());
            AreEqual(2, typed.Keys().Count());
            AreEqual(1, typed.Keys(only: new[] { "Key1" }).Count());
            AreEqual(0, typed.Keys(only: new[] { "Nonexisting" }).Count());
        }

    }
}
