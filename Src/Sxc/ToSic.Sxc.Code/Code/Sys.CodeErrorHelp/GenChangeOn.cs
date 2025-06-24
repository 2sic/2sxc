using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class GenChangeOn(string fullNamespace, string name, (string Code, string? Comment)[] alt)
    : GenNotExist(name, alt)
{

    public GenChangeOn(string fullNamespace, string name, string alt) : this(fullNamespace, name, [(alt, null)])
    { }

    public readonly string FullNameSpace = fullNamespace;
    public string? MsgWhichWasCommon;
    public string? NotOn;

    protected override string HtmlRecommendations() => Alt.Length == 1
        ? HtmlRec(("." + Alt[0].Code, Alt[0].Comment))
        : $"<ol>{string.Join("\n", Alt.Select(a => HtmlRec(("." + a.Code, a.Comment))))}</ol>";

    public override CodeHelp Generate()
    {
        return new()
        {
            Name = $"{FullNameSpace}-{Name}-DoesNotExist",
            Detect = DetectTypeDoesNotContain(FullNameSpace, Name),
            LinkCode = LinkCode,
            UiMessage = $@"
You are calling the '{Name}' property {MsgWhichWasCommon}, but not available on {NotOn} (RazorTyped). {Comments}
You should probably use '{Alt[0].Code}' {Alt[0].Comment}
",
            DetailsHtml = $@"
You are probably calling <code>.{Name}</code>.
{(Comments.HasValue() ? $"<br><em>{Comments}</em><br>" : "")}
The property <code>.{Name}</code> is replaced with: 
{HtmlRecommendations()}
"
        };

    }

}