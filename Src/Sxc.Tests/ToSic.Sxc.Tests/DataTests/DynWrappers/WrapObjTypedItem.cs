using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItem : DynAndTypedTestsBase
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

            var typed = Obj2Item(anon);
            IsNull(typed.Entity);
            IsNull(typed.Type);
            IsNull(typed.Presentation);
            IsNull(typed.Child("DoesntExist"));
        }

        [TestMethod] public void IsDemoItemDefault() => IsFalse(Obj2Item(new { }).IsDemoItem);
        [TestMethod] public void IsDemoItemFalse() => IsFalse(Obj2Item(new { IsDemoItem = false }).IsDemoItem);
        [TestMethod] public void IsDemoItemTrue() => IsTrue(Obj2Item(new { IsDemoItem = true }).IsDemoItem);

        [TestMethod] public void IdDefault() => AreEqual(0, Obj2Item(new { }).Id);
        [TestMethod] public void Id27() => AreEqual(27, Obj2Item(new { Id = 27 }).Id);
        [TestMethod] public void Id27Case() => AreEqual(27, Obj2Item(new { ID = 27 }).Id);

        [TestMethod] public void GuidDefault() => AreEqual(Guid.Empty, Obj2Item(new { }).Guid);
        [TestMethod] public void GuidCustom() => AreEqual(_guid, Obj2Item(new { Guid = _guid }).Guid);
        private static readonly Guid _guid = Guid.NewGuid();
        [TestMethod] public void TitleDefault() => IsNull(Obj2Item(new { }).Title);
        [TestMethod] public void TitleCustom() => AreEqual("title X", Obj2Item(new { Title = "title X" }).Title);
        [TestMethod] public void TitleNumber() => AreEqual("222", Obj2Item(new { Title = 222 }).Title);
        
    }
}
