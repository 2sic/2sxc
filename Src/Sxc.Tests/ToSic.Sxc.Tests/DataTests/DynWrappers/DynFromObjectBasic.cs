using System;
using System.Linq;
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

            IsTrue(typed.ContainsKey("Name"));
            IsTrue(typed.ContainsKey("NAME"));
            IsTrue(typed.ContainsKey("Description"));
            IsFalse(typed.ContainsKey("NonexistingField"));
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

            IsTrue(typed.ContainsKey("Name"));
            IsTrue(typed.ContainsKey("NAME"));
            IsTrue(typed.ContainsKey("Description"));
            IsFalse(typed.ContainsKey("NonexistingField"));
        }

        [TestMethod]
        public void Keys()
        {
            var anon = new
            {
                Key1 = "hello",
                Key2 = "goodbye"
            };
            var typed = GetService<DynamicWrapperFactory>().FromObject(anon, false, false) as ITyped;
            IsTrue(typed.ContainsKey("Key1"));
            IsFalse(typed.ContainsKey("Nonexisting"));
            IsTrue(typed.Keys().Any());
            AreEqual(2, typed.Keys().Count());
            AreEqual(1, typed.Keys(only: new[] { "Key1" }).Count());
            AreEqual(0, typed.Keys(only: new[] { "Nonexisting" }).Count());
        }
    }
}
