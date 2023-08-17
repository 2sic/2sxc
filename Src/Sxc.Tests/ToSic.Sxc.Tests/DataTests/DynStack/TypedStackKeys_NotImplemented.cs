using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynStack
{
    [TestClass]
    public class TypedStackKeys_NotImplemented: DynAndTypedTestsBase
    {
        public static IEnumerable<object[]> KeysAndExpectations => TypedStackTestData.KeysAndExpectations;

        [TestMethod]
        [DynamicData(nameof(KeysAndExpectations))]
        [ExpectedException(typeof(NotImplementedException))]
        public void Keys_AnonObjects(string key, bool expected) => AreEqual(expected, StackForKeysFromAnon.ContainsKey(key));


        private ITypedStack StackForKeysFromAnon => _stackForKeys ?? (_stackForKeys = TypedStackTestData.GetStackForKeysUsingAnon(this));
        private ITypedStack _stackForKeys;


    }
}
