using System.Diagnostics;
using ToSic.Sxc.Services;
using Xunit.Abstractions;
using static ToSic.Sxc.Services.SecureDataService;
#pragma warning disable xUnit1026

namespace ToSic.Sxc.ServicesTests.SecureData;

public class SecureDataTest(ISecureDataService sds, ITestOutputHelper output)
{
    private ISecureDataService GetSecureDataService()
    {
        sds.Debug = true;
        return sds;
    }

    //[Fact]
    //public void TestEncryptGoogleApiKey()
    //{
    //    var svc = GetSecureDataService() as SecureDataService;

    //    var newSecure = svc.Create(NewApiKeyV20);
    //    output.WriteLine($"New: '{newSecure}'");

    //    var secured = svc.Parse(SecuredApiKeyV19);
    //    Equal(OriginalApiKeyV19, secured.Value);
    //    Equal(SecuredApiKeyV19, svc.Create(OriginalApiKeyV19));
    //}

    private const string OriginalApiKeyV19 = "todo";
    private const string SecuredApiKeyV19 = "secure:pycbhspVSBHE662IjdEfFG8rwwCdxN9jCQaMJK6/QfLl/JxaDhAk+6q1WU4BSXw4;iv:HUyYDwdMhsuiaxZo3TG4Zg==";
    private const string NewApiKeyV20 = "todo";

    private const string TestGoogleApiKey = "Made-Up-Google-Maps-Key3423";
    private const string EncryptedApiKey = /*"secure:" +*/ "vOo8fn0SVDlt7GCtUrIpm120zrdufrR94WA3QsUUWiU=;iv:CQ7Rq+9p9CvWEtTeh8uhlA==";

    [Theory]
    [InlineData("No secret", "Test", "Test", false)]
    [InlineData("No secret with Prefix", PrefixSecure + "Test", PrefixSecure + "Test", false)]
    [InlineData("No secret with different cased prefix", "SECure:Test", "SECure:Test", false)]
    [InlineData("Google Api - no prefix, treat as not encrypted", TestGoogleApiKey, TestGoogleApiKey, false)]
    [InlineData("Google Api - with Prefix", TestGoogleApiKey, PrefixSecure + EncryptedApiKey)]
    [InlineData("Google Api - prefix lower case", TestGoogleApiKey, "secure:" + EncryptedApiKey)]
    public void TestDecryptPrefixes(string testName, string? expected, string original, bool mustBeSecure = true)
    {
        expected ??= original;
        var sds = GetSecureDataService() as SecureDataService;
        var result = sds.Parse(original);
        Trace.WriteLine($"Test: '{testName}'; Result: '{result.Value}'");
        Equal(expected, result.Value);
        Equal(mustBeSecure, result.IsSecured);//, "Must be Secure");
    }

    //[InlineData("test translate", "Secure:OaB/h/7jUZomSeEvhfFUhCvmrsH6XfmDykIpgXH9JBf6SxoTPY3FjW6O3PebeZ8X")]
    //[InlineData("test maps", "Secure:YPIieaLHdWhoeI1q0ULQv4WjGQMce2dKZ2apW7IVumwTzAap9LLJ4dmBQfDABC1g")]
    //[InlineData("test recaptcha Private", "Secure:OXtlYBVC2ijMLQqrev2XQCO9vwL12YmACxBh9JsWm6UE1H59Iehl93AmDRqZ3xId")]
    //[InlineData("test recaptcha Site Key public", "Secure:RVJvPMj+l9MJPTZhER0w/1P3kYD0+QYcoBZ4BZOAkpNYBxRBzfqcvl/SvfM5517T")]
    //[Fact]
    //public void TestDecrypt(string testName, string original)
    //{
    //    var sds = GetSecureDataService();
    //    ((SecureDataService)sds).Aes.TempUseNewMode = false;
    //    var normal = sds.Parse(original);
    //    Assert.Equal(-1, normal.Value.IndexOf("secure:", InvariantCultureIgnoreCase));
    //    var expected = normal.Value;
    //    Trace.WriteLine($"Test: '{testName}'; Result: '{normal.Value}'");

    //    Trace.WriteLine("Will re-encrypt new with OLD 2 iterations");
    //    var encrypted = sds.Create(normal.Value);
    //    Trace.WriteLine($"Re-encrypted {testName} '{encrypted}'");
    //    var reDecrypted = sds.Parse(encrypted);
    //    Assert.Equal(expected, reDecrypted.Value);

    //    Trace.WriteLine("Now with new key and 1000 iterations");
    //    sds = GetSecureDataService(); //.Aes.TempUseNewMode = true;
    //    encrypted = sds.Create(normal.Value);
    //    Trace.WriteLine($"Re-encrypted {testName} '{encrypted}'");
    //    reDecrypted = sds.Parse(encrypted);
    //    Assert.Equal(expected, reDecrypted.Value);
    //}

    [Theory]
    [InlineData("Google Maps v15.01", "secure:8Y3oOFHkGH0iMbZOsWfuqWnim7z1OgoNPrLsMZ/llCIhRJG+kUtDGC1m99v4GAlT;iv:jvZcqtq64R7eYzEFIphnzw==")]
    [InlineData("Google Translate v15.01", "secure:1mVLM8QnaXWvW/HgUXMk5wdNeXRXF6i59+5/wf19+yFU4AqZYRNPfcvufV3ObTf2;iv:Rb50nlPbqsz87QThp7BTWA==")]
    [InlineData("Google Recaptcha Private v15.01", "secure:WrrPM8F712DXIRuBe/eFW87L1AcrMKxPLFEJOJjqdFRZWNEPPRA2eHyz5yWIuF7N;iv:fIMctbzYuZFPQw0X151CLA==")]
    [InlineData("Google Recaptcha Site Key v15.01", "secure:fcU5EQMMf8tgoJG7VuR0nXvVKJNBsjSsnZPtctu1u45qb0j7Fsjuopvp6SImfWBw;iv:sNhfaORhKo4BT89qXGrlKw==")]
    public void TestBuiltInSecrets(string testName, string original)
    {
        var sds = GetSecureDataService() as SecureDataService;
        var normal = sds.Parse(original);
        Trace.WriteLine($"Test: '{testName}'; Result: '{normal.Value}'");
        Trace.WriteLine("Will re-encrypt new");
        var encrypted = sds.Create(normal.Value);
        Trace.WriteLine($"Re-encrypted {testName} '{encrypted}'");
        var reDecrypted = sds.Parse(encrypted);
        Equal(normal.Value, reDecrypted.Value);

        Trace.WriteLine("Now with new key");
        encrypted = sds.Create(normal.Value);
        Trace.WriteLine($"Re-encrypted {testName} '{encrypted}'");
        reDecrypted = sds.Parse(encrypted);
        Equal(normal.Value, reDecrypted.Value);
    }


    /// <summary>
    /// This doesn't actually test anything, but we can use it to get encrypted values for further testing
    /// and to encrypt stuff we need for our basic encrypted settings
    /// </summary>
    [Theory]
    [InlineData("Test Message 1", "Test Message - Original")]
    [InlineData("Test Message 2", "1234567890")]
    [InlineData("Test Made-Up Maps Key", "Made-Up-Google-Maps-Key3423")]
    public void TestRoundTrip(string testName, string value)
    {
        var secDataService = GetSecureDataService();
        var encrypted = ((SecureDataService)secDataService).Create(value);
        Trace.WriteLine("Encrypted: " + encrypted);

        var decrypted = secDataService.Parse(encrypted);
        Equal(value, decrypted.Value);
        Trace.WriteLine($"Parsed/Restored: '{decrypted.Value}'");
        Trace.WriteLine("Log Dump");
        Trace.WriteLine(secDataService.Log.Dump());
    }

}