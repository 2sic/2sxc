using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Services.PageService;

public class MockPageService: IPageService
{
    public string Activate(params string[] keys) => "";

    public string Activate(NoParamOrder noParamOrder = default, bool condition = true, params string[] features) => "";

    public IRawHtmlString AssetAttributes(NoParamOrder noParamOrder = default, bool optimize = true, int priority = 0,
        string position = null, bool whitelist = true) =>
        new RawHtmlString("");

    public string AddCsp(string name, params string[] values) => "";

    public string SetBase(string url = null) => "";

    public string SetTitle(string value, string placeholder = null) => "";

    public string SetDescription(string value, string placeholder = null) => "";

    public string SetKeywords(string value, string placeholder = null) => "";

    public string SetHttpStatus(int statusCode, string message = null) => "";

    public string AddToHead(string tag) => "";

    public string AddMeta(string name, string content) => "";

    public string AddOpenGraph(string property, string content) => "";

    public string AddJsonLd(string jsonString) => "";

    public string AddJsonLd(object jsonObject) => "";

    public string AddIcon(string path, NoParamOrder noParamOrder = default, string rel = "", int size = 0, string type = null) => "";

    public string AddIconSet(string path, NoParamOrder noParamOrder = default, object favicon = null, IEnumerable<string> rels = null,
        IEnumerable<int> sizes = null) =>
        "";

    public List<string> FeatureKeysAdded { get; }

    public string TurnOn(object runOrSpecs, NoParamOrder noParamOrder = default, object require = default, object data = default,
        IEnumerable<object> args = default, bool condition = true, bool? noDuplicates = default, string addContext = default) =>
        "";

    public string AddToHead(IHtmlTag tag) => "";
}