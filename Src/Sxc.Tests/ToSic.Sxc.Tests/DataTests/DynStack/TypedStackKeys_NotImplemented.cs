﻿using System;
using ToSic.Sxc.Data;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests.DataTests.DynStack;

[TestClass]
public class TypedStackKeys_NotImplemented: DynAndTypedTestsBase
{
    private static IEnumerable<object[]> StackProps => TypedStackTestData.StackOrder12PropInfo.ToTestEnum();

    [TestMethod]
    [DynamicData(nameof(StackProps))]
    [ExpectedException(typeof(NotImplementedException))]
    public void Keys_AnonObjects(PropInfo pti) => AreEqual(pti.Exists, StackForKeysFromAnon.ContainsKey(pti.Name));


    private ITypedStack StackForKeysFromAnon => _stackForKeys ??= TypedStackTestData.GetStackForKeysUsingAnon(this);
    private ITypedStack _stackForKeys;


}