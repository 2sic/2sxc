using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.Templates;

public class TemplatesEmptyTests(ITemplateService templateSvc, TemplatesTestsBaseHelper helper)  //: TemplatesTestsBase
{

    [Fact]
    public void EmptyServiceIsReused()
    {
        var instance1 = templateSvc.Empty();
        var instance2 = templateSvc.Empty();
        Equal(instance1, instance2);
    }


    [Theory]
    [InlineData("Hello World")]
    public void EmptyNoData(string expectedAndValue)
    {
        var svc = templateSvc;
        var result = svc.Empty().Parse(expectedAndValue);
        Equal(expectedAndValue, result);
    }


}