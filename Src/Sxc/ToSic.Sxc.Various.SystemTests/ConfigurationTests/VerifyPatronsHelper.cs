using ToSic.Sxc.Configuration.Internal;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Capabilities.Licenses;

namespace ToSic.Sxc.ConfigurationTests;

public class VerifyPatronsHelper(ILicenseService licenses, ISysFeaturesService features)
{
    /// <summary>
    /// A specific amount of licenses should be enabled.
    /// If the number changes, this test may need update.
    /// </summary>
    /// <param name="expected"></param>
    public void VerifyEnabledLicenses(int expected)
        => Equal(expected, licenses.Enabled.Count);


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