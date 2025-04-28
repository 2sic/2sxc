using ToSic.Eav.Internal.Features;
using ToSic.Eav.Internal.Licenses;
using ToSic.Sxc.Configuration.Internal;

namespace ToSic.Sxc.ConfigurationTests;

public class VerifyPatronsHelper(ILicenseService licenses, IEavFeaturesService features)
{
    public void VerifyPackageOk(int expected)
    {
        var result = licenses.Enabled;
        Equal(expected, result.Count);//, $"{expected} license package should be enabled. If the number changes, this test may need update.");
    }


    public void VerifyPatronPerfectionistsActive(bool expected)
    {
        var result = licenses.IsEnabled(BuiltInLicenses.PatronPerfectionist);

        // Our current test enables 6 packages, so the service should report so many active licenses
        Equal(expected, result);//, "Patron Perfectionist should be enabled?");
    }

    public void VerifyImageFormats(bool expected)
    {
        var result = features.IsEnabled(SxcFeatures.ImageServiceMultiFormat);

        // Our current test enables 6 packages, so the service should report so many active licenses
        Equal(expected, result);//, "Patron Perfectionist should be enabled");
    }
    
}