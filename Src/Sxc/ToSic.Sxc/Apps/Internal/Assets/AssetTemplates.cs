using ToSic.Lib.Services;

namespace ToSic.Sxc.Apps.Internal.Assets;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class Purpose
{
    // TODO: Once the UIs are updated, we'll kill the 'auto' case
    public const string Auto = "auto";
    //public const string Razor = "razor";
    //public const string Token = "token";
    //public const string Api = "api";
    //public const string Search = "search";
}

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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