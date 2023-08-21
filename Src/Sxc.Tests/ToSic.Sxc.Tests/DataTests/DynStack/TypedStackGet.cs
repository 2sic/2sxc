using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data;
using ToSic.Lib.Data;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynStack
{
    [TestClass]
    public class TypedStackGet: DynAndTypedTestsBase
    {
        private ITypedStack StackForKeysFromAnon => _stackForKeys ?? (_stackForKeys = TypedStackTestData.GetStackForKeysUsingAnon(this));
        private static ITypedStack _stackForKeys;


        private static IEnumerable<object[]> StackProps => TypedStackTestData.StackOrder12PropInfo.ToTestEnum();

        [TestMethod]
        [DynamicData(nameof(StackProps))]
        public void IsNotEmpty_BasedOnAnon(PropInfo pti)
        {
            AreEqual(pti.HasData, StackForKeysFromAnon.IsNotEmpty(pti.Name), pti.ToString());
        }

        [TestMethod]
        [DynamicData(nameof(StackProps))]
        public void Get_AnonObjects_ReqFalse(PropInfo pti) 
            => AreEqual(pti.Exists, StackForKeysFromAnon.Get(pti.Name, required: false) != null);

        [TestMethod]
        public void StackFromAnonWrapsObjectTyped()
        {
            var inspect = (IWrapper<IPropertyStack>)StackForKeysFromAnon;
            IsInstanceOfType(inspect.GetContents().Sources.FirstOrDefault().Value, typeof(PreWrapObject));
        }

        #region Stack from Json

        private ITypedStack StackForKeysFromTyped => _stackForKeysTyped ?? (_stackForKeysTyped = TypedStackTestData.GetStackForKeysUsingJson(this));
        private ITypedStack _stackForKeysTyped;

        [TestMethod]
        [DynamicData(nameof(StackProps))]
        public void Get_TypedObjects_NewReqFalse(PropInfo pti) 
            => AreEqual(pti.Exists, StackForKeysFromTyped.Get(pti.Name, required: false) != null);

        [TestMethod]
        public void StackFromTypedWrapsJacket()
        {
            var inspect = StackForKeysFromTyped as IWrapper<IPropertyStack>;
            IsInstanceOfType(inspect.GetContents().Sources.FirstOrDefault().Value, typeof(PreWrapJsonObject));
        }


        #endregion

    }
}
