using System;
using System.Globalization;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Tests.ServicesTests;

/// <summary>
/// Note: there are not many tests here, because the true engine is in the EAV conversion system which is tested very thoroughly already
/// </summary>
[Startup(typeof(StartupSxcCoreOnly))]
public class ConvertServiceTest(IConvertService convertSvc)
{
    private const string strGuid = "424e56ce-570a-4747-aee2-44c04caf7f12";
    private static readonly Guid expGuid = new(strGuid);
    [Fact] public void ToGuidNull() => Equal(Guid.Empty, Csvc().ToGuid(null));
    [Fact] public void ToGuidEmpty() => Equal(Guid.Empty, Csvc().ToGuid(""));
    [Fact] public void ToGuidBasic() => Equal(expGuid, Csvc().ToGuid(strGuid));
    [Fact] public void ToGuidFallback() => Equal(expGuid, Csvc().ToGuid("", expGuid));

    [Fact]
    public void ForCodeDate() 
        => Equal("2021-09-29T08:45:59.000z", Csvc().ForCode(new DateTime(2021, 09, 29, 08, 45, 59)));

    [Fact]
    public void ForCodeBool()
    {
        Equal("true", Csvc().ForCode(true));
        NotEqual(true.ToString(), Csvc().ForCode(true));
        Equal("false", Csvc().ForCode(false));
    }

    [Fact]
    public void ForCodeNumberBadCulture()
    {
        // Now change threading culture to a comma-culture and verify that change happened
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
        Equal("1,11", 1.11.ToString());

        // Now run tests expecting things to "just-work"
        var conv = Csvc();
        Equal("0", conv.ForCode("0"));
        Equal("1.11", conv.ForCode(1.11f));
        Equal("27.42", conv.ForCode(27.42));
        Equal("-27.42", conv.ForCode(-27.42));
    }

    /// <summary>
    /// test accessor
    /// </summary>
    /// <returns></returns>
    private IConvertService Csvc() => convertSvc;// GetService<IConvertService>();
}