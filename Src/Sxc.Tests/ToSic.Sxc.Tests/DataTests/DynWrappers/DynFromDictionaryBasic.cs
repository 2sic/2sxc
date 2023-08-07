using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class DynFromDictionaryBasic: TestBaseSxcDb
    {
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

            var typed = GetService<DynamicWrapperFactory>().FromDictionary(dic); // as ITyped;
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

    }
}
