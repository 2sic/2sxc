using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.Templates;

/// <summary>
/// Run tests on the Default() templates - ATM can't work, because the App doesn't exist, which is used to get the Default() template.
/// </summary>
public class TemplatesDefaultTests(ITemplateService templateSvc, TemplatesTestsBaseHelper helper) //: TemplatesTestsBase
{
    [Fact]
    public void DefaultServiceIsReused()
    {
        var instance1 = templateSvc.Default();
        var instance2 = templateSvc.Default();
        Equal(instance1, instance2);
    }
}