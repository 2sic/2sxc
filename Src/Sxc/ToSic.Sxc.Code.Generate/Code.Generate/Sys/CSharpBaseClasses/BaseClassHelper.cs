using ToSic.Lib.Logging;

namespace ToSic.Sxc.Code.Generate.Sys.CSharpBaseClasses;

internal class BaseClassHelper
{
    internal static (CSharpGeneratorHelper CSharpGenHelper, CodeFragment AppSnip, List<string> Usings) BaseClassTools(CSharpCodeSpecs cSharpSpecs, ILog parentLog)
    {
        var l = parentLog.Fn();
        var codeGenHelper = new CSharpGeneratorHelper(cSharpSpecs, parentLog);

        var snipApp = AppPropertyCodeFragment(codeGenHelper, parentLog);

        var allUsings = snipApp.Usings
            .Distinct()
            .OrderBy(u => u)
            .Select(u => $"using {u};")
            .ToList();

        l.Done();
        return (codeGenHelper, snipApp, allUsings);
    }

    private static CodeFragment AppPropertyCodeFragment(CSharpGeneratorHelper codeGenHelper, ILog parentLog)
    {
        var l = parentLog.Fn<CodeFragment>();
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

        return l.Return(snipApp);
    }

}