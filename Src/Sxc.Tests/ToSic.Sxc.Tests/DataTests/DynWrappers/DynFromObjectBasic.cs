using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class DynFromObjectBasic: TestBaseSxcDb
    {
        [TestMethod]
        public void BasicUseWithAnonymous()
        {
            var anon = new
            {
                Name = "2sxc",
                Description = "Some description",
                Founded = 2012,
                Birthday = new DateTime(2012, 5, 4),
                Truthy = true,
            };

            var typed = GetService<DynamicWrapperFactory>().FromObject(anon, false, false) as ITyped;
            dynamic dynAnon = typed;

            IsNull(dynAnon.NotExisting);
            AreEqual(anon.Name, dynAnon.Name);
            AreEqual(anon.Name, dynAnon.naME, "Should be the same irrelevant of case");
            AreEqual(anon.Birthday, dynAnon.Birthday, "dates should be the same");
            AreEqual(anon.Truthy, dynAnon.truthy);

            IsTrue(typed.Has("Name"));
            IsTrue(typed.Has("NAME"));
            IsTrue(typed.Has("Description"));
            IsFalse(typed.Has("NonexistingField"));
        }

        class AnonTyped
        {
            public string Name { get; set; }
            public string Description { get; set; }
            /// <summary> This one is not a real property but just a value! </summary>
            public string DescriptionAsProperty;
            public int Founded { get; set; }
            public DateTime Birthday { get; set; }
            public bool Truthy { get; set; }
        }

        [TestMethod]
        public void BasicUseWithTyped()
        {
            var anon = new AnonTyped
            {
                Name = "2sxc",
                Description = "Some description",
                DescriptionAsProperty = "Some description",
                Founded = 2012,
                Birthday = new DateTime(2012, 5, 4),
                Truthy = true,
            };

            var typed = GetService<DynamicWrapperFactory>().FromObject(anon, false, false) as ITyped;
            dynamic dynAnon = typed;

            IsNull(dynAnon.NotExisting);
            AreEqual(anon.Name, dynAnon.Name);
            AreEqual(anon.Name, dynAnon.naME, "Should be the same irrelevant of case");
            // diff to previous test
            AreNotEqual(anon.DescriptionAsProperty, dynAnon.DescriptionAsProperty, "Should NOT work for values, only properties");
            AreEqual(null, dynAnon.DescriptionAsProperty, "Should NOT work for values, only properties");
            AreEqual(anon.Birthday, dynAnon.Birthday, "dates should be the same");
            AreEqual(anon.Truthy, dynAnon.truthy);

            IsTrue(typed.Has("Name"));
            IsTrue(typed.Has("NAME"));
            IsTrue(typed.Has("Description"));
            IsFalse(typed.Has("NonexistingField"));
        }
    }
}
