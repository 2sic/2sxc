using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItem : DynWrapperTestBase
    {

        [TestMethod]
        public void PropsWhichDefaultToNull()
        {
            var anon = new
            {
                Simple = "simple",
                Sub = new
                {
                    SubSub = "Test"
                }
            };

            var typed = ItemFromObject(anon);
            IsNull(typed.Entity);
            IsNull(typed.Type);
            IsNull(typed.Presentation);
            IsNull(typed.Child("DoesntExist"));
            IsNull(typed.Field("Simple"));
        }

        [TestMethod] public void IsDemoItemDefault() => IsFalse(ItemFromObject(new { }).IsDemoItem);
        [TestMethod] public void IsDemoItemFalse() => IsFalse(ItemFromObject(new { IsDemoItem = false }).IsDemoItem);
        [TestMethod] public void IsDemoItemTrue() => IsTrue(ItemFromObject(new { IsDemoItem = true }).IsDemoItem);

        [TestMethod] public void IdDefault() => AreEqual(0, ItemFromObject(new { }).Id);
        [TestMethod] public void Id27() => AreEqual(27, ItemFromObject(new { Id = 27 }).Id);
        [TestMethod] public void Id27Case() => AreEqual(27, ItemFromObject(new { ID = 27 }).Id);

        [TestMethod] public void GuidDefault() => AreEqual(Guid.Empty, ItemFromObject(new { }).Guid);
        [TestMethod] public void GuidCustom() => AreEqual(_guid, ItemFromObject(new { Guid = _guid }).Guid);
        private static readonly Guid _guid = Guid.NewGuid();
        [TestMethod] public void TitleDefault() => IsNull(ItemFromObject(new { }).Title);
        [TestMethod] public void TitleCustom() => AreEqual("title X", ItemFromObject(new { Title = "title X" }).Title);
        [TestMethod] public void TitleFromName() => AreEqual("title X", ItemFromObject(new { Name = "title X" }).Title);
        [TestMethod] public void TitleNumber() => AreEqual("222", ItemFromObject(new { Title = 222 }).Title);
        
    }
}
