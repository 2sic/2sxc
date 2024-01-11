using ToSic.Eav.Code.Help;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class GenNotExist(string name, (string Code, string Comment)[] alt)
{
    public GenNotExist(string name, params string[] alt) : this(name, alt?.Select(r => (r, null as string)).ToArray()) { }
    public GenNotExist(string name, (string Code, string Comment) alt) : this(name, [alt]) { }

    protected virtual string HtmlRecommendations() => Alt.Length == 1
        ? HtmlRec(Alt[0])
        : $"<ol>{string.Join("\n", Alt.Select(HtmlRec))}</ol>";

    public readonly string Name = name;
    public string Comments;
    public (string Code, string Comment)[] Alt = alt.SafeAny() ? alt : new (string, string)[] { ("unknown", null) };
    public string LinkCode;
    public string MsgNotSupportedIn;

    public virtual CodeHelp Generate()
    {
        var recHtml = HtmlRecommendations();
        return new(name: $"Object-{Name}-DoesNotExist",
            detect: $"error CS0103: The name '{Name}' does not exist in the current context",
            linkCode: LinkCode,
            uiMessage: $@"
You are calling the '{Name}' object which {MsgNotSupportedIn}. {Comments}
You should probably use '{Alt[0].Code}' {Alt[0].Comment}
",
            detailsHtml: $@"
You are probably calling <code>{Name}</code>.
{(Comments.HasValue() ? $"<br><em>{Comments}</em><br>" : "")}
The property <code>{Name}</code> {MsgNotSupportedIn}. 
Probably better: 
{recHtml}
"
        );
    }

    /// <summary>
    /// Build detection string for errors which say ... does not contain...
    /// </summary>
    internal static string DetectTypeDoesNotContain(string typeName, string property) =>
        $"error CS1061: '{typeName}' does not contain a definition for '{property}' and no extension method '{property}' accepting a first argument of type '{typeName}' could be found";

    protected static string HtmlRec((string Code, string Comment) r)
        => $"<li>{(r.Comment.HasValue() ? r.Comment + " - " : "")}<code>{r.Code}</code></li>";
}