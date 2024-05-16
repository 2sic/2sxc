using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.ServicesTests.Templates;

/// <summary>
/// Run tests on the Default() templates - ATM can't work, because the App doesn't exist, which is used to get the Default() template.
/// </summary>
[TestClass]
public class TemplatesDefaultTests: TemplatesTestsBase
{
    [TestMethod]
    public void DefaultServiceIsReused()
    {
        var svc = GetTemplateServices();
        var instance1 = svc.Default();
        var instance2 = svc.Default();
        Assert.AreEqual(instance1, instance2);
    }
}