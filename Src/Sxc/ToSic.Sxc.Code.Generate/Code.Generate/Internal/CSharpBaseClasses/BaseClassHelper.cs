namespace ToSic.Sxc.Code.Generate.Internal.CSharpBaseClasses;

internal class BaseClassHelper
{
    internal static CodeFragment AppPropertyCodeFragment(CSharpGeneratorHelper codeGenHelper)
    {
        var tabs = codeGenHelper.Specs.TabsProperty;
        var indent = codeGenHelper.Indent(tabs);
        var summary = codeGenHelper.XmlComment(tabs, ["Typed App with typed Settings & Resources"]);
        var snipApp = new CodeFragment(
            "App",
            summary +
            $"""
             {indent}public new IAppTyped<AppSettings, AppResources> App => _app ??= Customize.App<AppSettings, AppResources>();
             {indent}private IAppTyped<AppSettings, AppResources> _app;
             """,
            true,
            ["AppCode.Data", "ToSic.Sxc.Apps"]
        );
        return snipApp;
    }


    internal static (CSharpGeneratorHelper CSharpGenHelper, CodeFragment AppSnip, List<string> Usings) BaseClassTools(CSharpCodeSpecs cSharpSpecs)
    {
        var codeGenHelper = new CSharpGeneratorHelper(cSharpSpecs);

        var snipApp = AppPropertyCodeFragment(codeGenHelper);

        var allUsings = snipApp.Usings
            .Distinct()
            .OrderBy(u => u)
            .Select(u => $"using {u};")
            .ToList();

        return (codeGenHelper, snipApp, allUsings);
    }
}