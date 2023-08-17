using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data;
using ToSic.Lib.Data;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynStack
{
    [TestClass]
    public class TypedStackGet: DynAndTypedTestsBase
    {
        public static IEnumerable<object[]> KeysAndExpectations => TypedStackTestData.KeysAndExpectations;

        private ITypedStack StackForKeysFromAnon => _stackForKeys ?? (_stackForKeys = TypedStackTestData.GetStackForKeysUsingAnon(this));
        private ITypedStack _stackForKeys;

        [TestMethod]
        [DynamicData(nameof(KeysAndExpectations))]
        public void Get_AnonObjects(string key, bool expected)
        {
            AreEqual(expected, StackForKeysFromAnon.Get(key) != null);
        }

        [TestMethod]
        public void StackFromAnonWrapsObjectTyped()
        {
            var inspect = StackForKeysFromAnon as IWrapper<IPropertyStack>;
            IsInstanceOfType(inspect.GetContents().Sources.FirstOrDefault().Value, typeof(PreWrapObject));
        }

        #region Stack from Json

        private ITypedStack StackForKeysFromTyped => _stackForKeysTyped ?? (_stackForKeysTyped = TypedStackTestData.GetStackForKeysUsingJson(this));
        private ITypedStack _stackForKeysTyped;

        [TestMethod]
        [DynamicData(nameof(KeysAndExpectations))]
        public void Get_TypedObjects(string key, bool expected)
        {
            AreEqual(expected, StackForKeysFromTyped.Get(key) != null);
        }

        [TestMethod]
        public void StackFromTypedWrapsJacket()
        {
            var inspect = StackForKeysFromTyped as IWrapper<IPropertyStack>;
            IsInstanceOfType(inspect.GetContents().Sources.FirstOrDefault().Value, typeof(PreWrapJsonObject));
        }


        #endregion

    }
}
