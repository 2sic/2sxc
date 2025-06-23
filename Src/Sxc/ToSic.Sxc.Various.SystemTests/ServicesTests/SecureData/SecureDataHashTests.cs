using ToSic.Sxc.Services;
using ToSic.Sys.Security.Encryption;

namespace ToSic.Sxc.ServicesTests.SecureData;

/// <summary>
/// Test Hash functions on SecureData Service.
/// </summary>
/// <remarks>
/// Test data created using https://emn178.github.io/online-tools/sha256.html
/// </remarks>
[Startup(typeof(StartupSxcCoreOnly))]
public class SecureDataHashTests(ISecureDataService sds)
{
    private ISecureDataService GetSecureDataService()
    {
        sds.Debug = true;
        return sds;
    }

    [Fact]
    public void TestSha256RootWithNull() =>
        Throws<ArgumentNullException>(() => Sha256.Hash(null!));

    /// <remarks>
    /// test data created using https://emn178.github.io/online-tools/sha256.html
    /// </remarks>
    [Theory]
    [InlineData("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", "")]
    [InlineData("aef87954c3c56f1d53eaca2e1ae5f863bf1c0331e418bdf71e9e4c07085eebdd", "", "asc4y9is2!y")]
    [InlineData("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", (string?)null)]
    [InlineData("9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", "test")]
    [InlineData("532eaabd9574880dbf76b9b8cc00832c20a6ec113d682299550d7a6e0f345e25", "Test")]
    [InlineData("94ee059335e587e501cc4bf90613e0814f00a7b08bc7c648fd865a2af6a22cc2", "TEST")]
    [InlineData("2c9cc266a15dc85945c040defd12387b3fca49a2a6ef2cd49102b010e9bbc307", "TEST", "asc4y9is2!y")]
    public void TestSha256(string expected, string value, string? salt = default)
    {
        var sds = GetSecureDataService();
        var result = sds.HashSha256(value + salt);
        Equal(expected, result);//, $"failed on '{value}'; salt: '{salt}'");
    }

    /// <summary>
    /// Verify that incorrect values do not match the expected hash.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="value"></param>
    /// <param name="salt"></param>
    [Theory]
    [InlineData("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", ".")]
    [InlineData("aef87954c3c56f1d53eaca2e1ae5f863bf1c0331e418bdf71e9e4c07085eebdd", ".", "asc4y9is2!y")]
    public void TestSha256Invalid(string expected, string value, string? salt = default)
    {
        var sds = GetSecureDataService();
        var result = sds.HashSha256(value + salt);
        NotEqual(expected, result);//, $"failed on '{value}'; salt: '{salt}'");
    }


    /// <remarks>
    /// test data created using https://emn178.github.io/online-tools/sha512.html
    /// </remarks>
    [Theory]
    [InlineData("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e", "")]
    [InlineData("3578b7e749b86c2a7d2e38ad9bd29573edb96edee4b1a4367ab917321d8adac0965d16b9a440cc99cf50014be3d9cc904043ca5d531131e0b7ae34cae25b3788", "", "asc4y9is2!y")]
    [InlineData("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e", (string?)null)]
    [InlineData("ee26b0dd4af7e749aa1a8ee3c10ae9923f618980772e473f8819a5d4940e0db27ac185f8a0e1d5f84f88bc887fd67b143732c304cc5fa9ad8e6f57f50028a8ff", "test")]
    [InlineData("c6ee9e33cf5c6715a1d148fd73f7318884b41adcb916021e2bc0e800a5c5dd97f5142178f6ae88c8fdd98e1afb0ce4c8d2c54b5f37b30b7da1997bb33b0b8a31", "Test")]
    [InlineData("7bfa95a688924c47c7d22381f20cc926f524beacb13f84e203d4bd8cb6ba2fce81c57a5f059bf3d509926487bde925b3bcee0635e4f7baeba054e5dba696b2bf", "TEST")]
    [InlineData("b74814c96b1f0c76c4f1cc10aed08a0c56f25a2611aaf32f4560a8abcffe30ff2c7828a953d1c18e1f03a4fa1c335c84807bbedf7fb6b7da5b4b1e56d46a30d9", "TEST", "asc4y9is2!y")]
    public void TestSha512(string expected, string value, string? salt = default)
    {
        var sds = GetSecureDataService();
        var result = sds.HashSha512(value + salt);
        Equal(expected, result);//, $"failed on '{value}'; salt: '{salt}'");
    }

    /// <summary>
    /// Verify that incorrect values do not match the expected hash.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="value"></param>
    /// <param name="salt"></param>
    [Theory]
    [InlineData("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e", " ")]
    [InlineData("3578b7e749b86c2a7d2e38ad9bd29573edb96edee4b1a4367ab917321d8adac0965d16b9a440cc99cf50014be3d9cc904043ca5d531131e0b7ae34cae25b3788", " ", "asc4y9is2!y")]
    public void TestSha512Invalid(string expected, string value, string? salt = default)
    {
        var sds = GetSecureDataService();
        var result = sds.HashSha512(value + salt);
        NotEqual(expected, result);//, $"failed on '{value}'; salt: '{salt}'");
    }
}