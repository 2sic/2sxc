namespace ToSic.Sxc.Tests.ServicesTests.Templates;

[TestClass]
public class TemplatesEmptyTests: TemplatesTestsBase
{

    [TestMethod]
    public void EmptyServiceIsReused()
    {
        var svc = GetTemplateServices();
        var instance1 = svc.Empty();
        var instance2 = svc.Empty();
        AreEqual(instance1, instance2);
    }


    [DataRow("Hello World")]
    [TestMethod]
    public void EmptyNoData(string expectedAndValue)
    {
        var svc = GetTemplateServices();
        var result = svc.Empty().Parse(expectedAndValue);
        AreEqual(expectedAndValue, result);
    }


}