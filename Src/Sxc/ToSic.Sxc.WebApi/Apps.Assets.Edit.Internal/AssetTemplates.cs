namespace ToSic.Sxc.Apps.Internal.Assets;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class AssetTemplates() : ServiceBase("SxcAss.Templt")
{
    #region Constants

    internal const string ForTemplate = "Template";
    internal const string ForApi = "Api";
    internal const string ForDataSource = "DataSource";
    private const string ForDocs = "Documentation";
    public const string ForCode = "Code";
    public const string ForSearch = "Search";

    internal const string TypeRazor = "Razor";
    internal const string TypeToken = "Token";
    internal const string TypeNone = "";


    #endregion


    public List<TemplateInfo> GetTemplates() => _templates ??=
    [
        RazorTyped,
        RazorHybrid,
        RazorDnn,
        CsTyped,
        CsHybrid,
        // DnnCsCode,
        ApiTyped,
        ApiHybrid,
        // DataSourceTyped,
        DataSourceHybrid,
        Token,
        DnnSearch,
        Markdown,
        EmptyTextFile,
        EmptyFile
    ];
    private static List<TemplateInfo> _templates;

    public const string CsApiTemplateControllerName = "PleaseRenameController";
    public const string CsDataSourceName = "PleaseRename";
}