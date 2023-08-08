using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItemParents : DynWrapperTestBase
    {
        private class TestTag
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        private static readonly TestTag[] TestTagList =
        {
            new TestTag
            {
                Id = 1001,
                Name = "Web",
            },
            new TestTag
            {
                Id = 1002,
                Name = "IT"
            }
        };


        private class TestDataParentsFlat
        {
            public TestTag[] Parents => TestTagList;
        }
        private static readonly TestDataParentsFlat DataParentFlat = new TestDataParentsFlat();

        private IEnumerable<ITypedItem> ParentsFlat => ItemFromObject(DataParentFlat).Parents();

        [TestMethod] public void ParentsFlatExists() => IsNotNull(ParentsFlat);
        [TestMethod] public void ParentsFlatCount() => AreEqual(2, ParentsFlat.Count());
        [TestMethod] public void ParentsFlat1Id() => AreEqual(DataParentFlat.Parents[0].Id, ParentsFlat.First().Id);
        [TestMethod] public void ParentsFlat1Name() => AreEqual(DataParentFlat.Parents[0].Name, ParentsFlat.First().Title);
        [TestMethod] public void ParentsFlat2Id() => AreEqual(DataParentFlat.Parents[1].Id, ParentsFlat.Skip(1).First().Id);

        private class TestDataParent
        {
            public object Parents { get; } = new
            {
                Tags = new[]
                {
                    new TestTag
                    {
                        Id = 1001,
                    },
                    new TestTag {
                        Id = 1002,
                    }
                }
            };
        }

        private IEnumerable<ITypedItem> Tags => ItemFromObject(DataParent).Parents("Tags");
        private static TestDataParent DataParent { get; } = new TestDataParent();
        [TestMethod] public void ParentTagsExists() => IsNotNull(Tags);
        [TestMethod] public void ParentTagsExists2() => AreEqual(2, Tags.Count());
        [TestMethod] public void ParentFakeEmptyList() => IsNotNull(ItemFromObject(DataParent).Parents("Fake"));
        [TestMethod] public void ParentFakeEmptyList0() => AreEqual(0, ItemFromObject(DataParent).Parents("Fake").Count());


        private class TestDataParents
        {
            public TestPerson[] Parents { get; } = {
                new TestPerson
                {
                    Id = 1001,
                },
                new TestPerson
                {
                    Id = 1002,
                }
            };
        }
        private static readonly TestDataParents DataParents = new TestDataParents();
    }
}
