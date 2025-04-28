using System.Globalization;
using ToSic.Sxc.Services;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.Templates;

public class TemplatesBasicTests(ITemplateService templateSvc, TemplatesTestsBaseHelper helper, ITestOutputHelper output)// : TemplatesTestsBase
{
    [InlineData("Hello World", "Hello World")]
    [InlineData("Hello Val1", "Hello [Dic:Key1]")]
    [InlineData("Hello Val2", "Hello [Dic:Key2]")]
    [InlineData("Hello ", "Hello [Dic:unknown]")]
    [InlineData("Hello There", "Hello [Dic:unknown||There]")]
    [InlineData("Hello Val2", "Hello [Dic:unknown||[Dic:Key2]]")]
    [InlineData("Hello ", "Hello [Unknown:Key1]")]
    [Theory]
    public void EmptyDictionary(string expected, string value, string notes = default)
    {
        var result = templateSvc.Empty(sources: [helper.GetDicSource()]).Parse(value);
        Equal(expected, result);//, $"Value: '{value}', notes: {notes}");
    }

    [InlineData("Hello World", "Hello World")]
    [InlineData("Hello Val1", "Hello [Fn:Key1]")]
    [InlineData("Hello Val2", "Hello [Fn:Key2]")]
    [InlineData("Hello WrappedVal1Done", "Hello [Fn:Key1|Wrapped{0}Done]")]
    [Theory]
    public void EmptyFunction(string expected, string value, string notes = default)
    {
        var result = templateSvc.Empty(sources: [helper.GetFnSource()]).Parse(value);
        Equal(expected, result);//, $"Value: '{value}', notes: {notes}");
    }

    [InlineData("Hello World", "Hello World")]
    [InlineData("Hello 42", "Hello [Fn:Key1]")]
    [InlineData("Hello 27", "Hello [Fn:Key2]")]
    [InlineData("Hello 42", "Hello [Fn:Key1|G]")]
    [InlineData("Hello 27", "Hello [Fn:Key2|G]")]
    [InlineData("Hello 42.00", "Hello [Fn:Key1|F]")]
    [InlineData("Hello 27.00", "Hello [Fn:Key2|F]")]
    [InlineData("Hello (42)", "Hello [Fn:Key1|(##)]")]
    [InlineData("Hello 42.00", "Hello [Fn:Key1|00.00]")]
    [Theory]
    public void EmptyFunctionFormatter(string expected, string value, string notes = default)
    {
        var culture = new CultureInfo("en-US")
        {
            // Workaround for .net 9 - in 472 this is the default, but in .net 9 it's 3 digits.
            // Not sure if we should also change the internals of 2sxc for other conversions,
            // here it only affects the unit test
            NumberFormat =
            {
                NumberDecimalDigits = 2
            }
        };

        output.WriteLine($"Culture: {culture.Name}; {culture.NumberFormat.CurrencyDecimalDigits}");
        Thread.CurrentThread.CurrentCulture = culture;
        var templateEngine = templateSvc.Empty(sources: [helper.GetFnNumberSourcesWithFormat()]);
        var result = templateEngine.Parse(value);
        Equal(expected, result);//, $"Value: '{value}', notes: {notes}");
    }

}