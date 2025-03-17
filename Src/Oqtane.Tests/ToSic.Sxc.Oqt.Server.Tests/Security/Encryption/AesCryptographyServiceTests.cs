
using ToSic.Eav.Security.Encryption;

namespace ToSic.Sxc.Oqt.Security.Encryption;

public class AesCryptographyServiceTests
{
    [Fact]
    public void DecryptTest()
    {
#pragma warning disable CS0219
        var x = "secure:pycbhspVSBHE662IjdEfFG8rwwCdxN9jCQaMJK6/QfLl/JxaDhAk+6q1WU4BSXw4;iv:HUyYDwdMhsuiaxZo3TG4Zg==";
        var v = "pycbhspVSBHE662IjdEfFG8rwwCdxN9jCQaMJK6/QfLl/JxaDhAk+6q1WU4BSXw4";
#pragma warning restore CS0219
        var r = new Rfc2898Generator();
        var aes = new AesCryptographyService(r);
        var ret = aes.DecryptFromBase64(v, new(true) { InitializationVector64 = "HUyYDwdMhsuiaxZo3TG4Zg==" } );
        Equal("AIzaSyAKEFBVw7SddUQR0YnAuTam5wpXvDomzts", ret);
    }
}