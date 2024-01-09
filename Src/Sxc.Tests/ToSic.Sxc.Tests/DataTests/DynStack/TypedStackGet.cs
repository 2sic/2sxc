using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data;
using ToSic.Lib.Data;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Tests.DataTests.DynStack.TypedStackTestData;

namespace ToSic.Sxc.Tests.DataTests.DynStack
{
    [TestClass]
    public class TypedStackGet: DynAndTypedTestsBase
    {
        private ITypedStack StackFromAnon => _stackForKeys ??= GetStackForKeysUsingAnon(this);
        private static ITypedStack _stackForKeys;

        private ITypedStack StackFromJson => _stackForKeysTyped ??= GetStackForKeysUsingJson(this);
        private ITypedStack _stackForKeysTyped;


        private static IEnumerable<object[]> StackProps => StackOrder12PropInfo.ToTestEnum();
        private static IEnumerable<object[]> StackPropsWithValue => StackOrder12PropInfo.Where(sp => sp.HasData && sp.Value?.ToString() != ValueNotTestable).ToTestEnum();


        [TestMethod]
        [DynamicData(nameof(StackProps))]
        public void IsNotEmpty_BasedOnAnon(PropInfo pti)
            => AreEqual(pti.HasData, StackFromAnon.IsNotEmpty(pti.Name), pti.ToString());

        [TestMethod]
        [DynamicData(nameof(StackProps))]
        public void ExistsNotNull_FromAnon_ReqFalse(PropInfo pti) 
            => AreEqual(pti.Exists, StackFromAnon.Get(pti.Name, required: false) != null);

        [TestMethod]
        [DynamicData(nameof(StackPropsWithValue))]
        public void ExistsNotNull_FromJson_ReqFalse(PropInfo pti)
            => AreEqual(pti.Exists, StackFromJson.Get(pti.Name, required: false) != null);

        [TestMethod]
        [DynamicData(nameof(StackProps))]
        public void IsNotEmpty_FromAnon_ReqFalse(PropInfo pti) 
            => AreEqual(pti.Exists, StackFromAnon.IsNotEmpty(pti.Name));

        [TestMethod]
        [DynamicData(nameof(StackPropsWithValue))]
        public void IsNotEmpty_FromJson_ReqFalse(PropInfo pti)
            => AreEqual(pti.Exists, StackFromJson.IsNotEmpty(pti.Name));

        [TestMethod]
        [DynamicData(nameof(StackPropsWithValue))]
        public void Get_FromJson_ReqFalse(PropInfo pti)
            => AreEqual(pti.Value, StackFromJson.Get(pti.Name, required: false));


        [TestMethod]
        [DynamicData(nameof(StackPropsWithValue))]
        public void Get_FromAnon_ReqFalse(PropInfo pti) 
            => AreEqual(pti.Value, StackFromAnon.Get(pti.Name, required: false));


        #region Verify that what's inside the object matches expectation

        [TestMethod]
        public void Verify_StackFromAnonWrapsObjectTyped()
        {
            var inspect = (IWrapper<IPropertyStack>)StackFromAnon;
            IsInstanceOfType(inspect.GetContents().Sources.FirstOrDefault().Value, typeof(PreWrapObject));
        }


        [TestMethod]
        public void Verify_StackFromJsonWrapsJacket()
        {
            var inspect = (IWrapper<IPropertyStack>)StackFromJson;
            IsInstanceOfType(inspect.GetContents().Sources.FirstOrDefault().Value, typeof(PreWrapJsonObject));
        }


        #endregion

    }
}
