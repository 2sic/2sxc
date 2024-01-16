using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;

// ReSharper disable ConvertTypeCheckPatternToNullCheck

namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ProblemSuggestions
{
    private const string Mobius5NameId = "ea777610-00e3-462f-8a3e-90a09a6e1109";
    private const string Blog6NameId = "72e406fd-500f-4632-82ca-942b22358b56";

    public IEnumerable<ProblemReport> AddSuggestions(IBlock block, List<Exception> exsOrNull, string errorCode)
    {
        var suggestions = new List<ProblemReport>();

        // Add suggestions for any exceptions in the code
        if (exsOrNull.SafeAny())
        {
            // deduplicate - in case we have many of the same errors
            var unique = exsOrNull
                .GroupBy(e => e.Message)
                .Select(g => g.First())
                .ToList();

            foreach (var ex in unique)
                if ((ex as IExceptionWithHelp)?.Helps is List<CodeHelp> helps)
                    helps.ForEach(h => suggestions.Add(new()
                    {
                        Link = h.Link.NullIfNoValue(),
                        Message = h.DetailsHtml ?? h.UiMessage,
                        Severity = ProblemReport.ErrorSeverity.warning,
                    }));
        }


        if (errorCode == null || block?.App == null) return suggestions;

        // Special suggestion for Blog v6.0.0/1
        AddWarning1601(block.App, "Blog", Blog6NameId, new(6, 0, 0), suggestions, "app-blog-upgrade601");
        AddWarning1601(block.App, "Blog", Blog6NameId, new(6, 0, 1), suggestions, "app-blog-upgrade601");

        // Special Suggestions for Mobius 5.7.0
        AddWarning1601(block.App, "Mobius", Mobius5NameId, new(5, 7, 0), suggestions, "app-mobius-upgrade570");

        return suggestions;
    }

    private static void AddWarning1601(IApp app, string appName, string nameId, Version version, List<ProblemReport> suggestions, string shortLink)
    {
        if (app.NameId != nameId || app.Configuration.Version.CompareTo(version) != 0) return;
        suggestions.Add(new()
        {
            Scope = ProblemReport.ErrorScope.app,
            Severity = ProblemReport.ErrorSeverity.error,
            Message = $"The {appName} App {version} used some very new APIs in 2sxc 16.01 which had to be revised in 16.02. Check this guide for what to fix.",
            Link = $"https://go.2sxc.org/{shortLink}"
        });
    }
}