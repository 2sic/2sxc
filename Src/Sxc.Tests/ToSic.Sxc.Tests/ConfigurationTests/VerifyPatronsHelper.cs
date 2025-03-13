using ToSic.Eav.Code;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Internal.Licenses;
using ToSic.Sxc.Configuration.Internal;

namespace ToSic.Sxc.Tests.ConfigurationTests;

class VerifyPatronsHelper(ICanGetService parent)
{
    public void VerifyPackageOk(int expected)
    {
        var licenses = parent.GetService<ILicenseService>();
        var result = licenses.Enabled;
        AreEqual(expected, result.Count, $"{expected} license package should be enabled. If the number changes, this test may need update.");
    }


    public void VerifyPatronPerfectionistsActive(bool expected)
    {
        var licenses = parent.GetService<ILicenseService>();
        var result = licenses.IsEnabled(BuiltInLicenses.PatronPerfectionist);

        // Our current test enables 6 packages, so the service should report so many active licenses
        AreEqual(expected, result, "Patron Perfectionist should be enabled?");
    }

    public void VerifyImageFormats(bool expected)
    {
        var features = parent.GetService<IEavFeaturesService>();
        var result = features.IsEnabled(SxcFeatures.ImageServiceMultiFormat);

        // Our current test enables 6 packages, so the service should report so many active licenses
        AreEqual(expected, result, "Patron Perfectionist should be enabled");
    }
    
}