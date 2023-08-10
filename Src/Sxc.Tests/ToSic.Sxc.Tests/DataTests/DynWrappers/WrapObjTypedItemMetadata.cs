using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItemMetadata : DynWrapperTestBase
    {
        private class TestDataMd1
        {
            public dynamic Metadata => new
            {
                Id = 999,
                Description = "MD Description"
            };
        }
        private ITypedItem ItemMd1 => ItemFromObject(new TestDataMd1());

        [TestMethod] public void MetadataHasValue() => IsNotNull(ItemMd1.Metadata);
        [TestMethod] public void MetadataCount1() => AreEqual(1, (ItemMd1.Metadata as IEnumerable<IDynamicEntity>).Count());
        //[TestMethod] public void MetadataId() => AreEqual(999, Item.Metadata.Entity.EntityId);
        [TestMethod] public void MetadataDescription() => AreEqual("MD Description", ItemMd1.Metadata.Get<string>("Description"));

        private class TestDataMd3
        {
            public dynamic Metadata => new object[]
            {
                new
                {
                    Id = 999,
                    Description = "MD3 Description"
                },
                new {
                    Id = 555,
                    Color = "white"
                },
                new {
                    Id = 555,
                    Color = "black"
                },
            };

        }
        private ITypedItem ItemMd3 => ItemFromObject(new TestDataMd3());
        [TestMethod] public void Metadata3HasValue() => IsNotNull(ItemMd3.Metadata);
        [TestMethod] public void Metadata3Count3() => AreEqual(3, (ItemMd3.Metadata as IEnumerable<IDynamicEntity>).Count());
        [TestMethod] public void Metadata3Description() => AreEqual("MD3 Description", ItemMd3.Metadata.Get<string>("Description"));
        [TestMethod] public void Metadata3Color() => AreEqual("white", ItemMd3.Metadata.Get<string>("Color"));

    }
}
