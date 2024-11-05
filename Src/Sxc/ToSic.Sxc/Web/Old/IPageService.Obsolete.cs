using ToSic.Razor.Blade;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Web;
// Important: There is a critical bug in Razor that methods which an interface inherits
// Will fail when called using dynamic parameters. 
// https://stackoverflow.com/questions/3071634/strange-behaviour-when-using-dynamic-types-as-method-parameters
// Because of this,
// - ToSic.Sxc.Web.IPageService.SetTitle("ok") works
// - ToSic.Sxc.Web.IPageService.SetTitle(dynEntity.Title) fails!!!
// This is why each method on the underlying interface must be repeated here :(
//
// We suggest that we won't do this for new commands, but all commands that were in 12.08 must be repeated here
// like Blog 06.00.01 which was still around early 2023 uses this, so it could also be used in Oqtane

/// <summary>
/// Old name for the IPageService, it's in use in some v12 App templates so we must keep it working.
/// Will continue to work, but shouldn't be used. Please use <see cref="ToSic.Sxc.Services.IPageService"/>  instead
/// </summary>
[PrivateApi]
[Obsolete("Use ToSic.Sxc.Services.IPageService instead")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPageService // : ToSic.Sxc.Services.IPageService
{
    // This repeats the definition on the IPage Service
    // For reasons we cannot explain, Razor sometimes otherwise complains
    // that a GetService<ToSic.Sxc.Web.IPageService>()
    // Doesn't contain this command
    // We don't know why - once this is added here everything works
    // So for now we just leave it in

#pragma warning disable CS0108, CS0114
    string SetBase(string url = null);
    string SetTitle(string value, string placeholder = null);
    string SetDescription(string value, string placeholder = null);
    string SetKeywords(string value, string placeholder = null);
    string SetHttpStatus(int statusCode, string message = null);
    string AddToHead(string tag);
    string AddToHead(IHtmlTag tag);
    string AddMeta(string name, string content);
    string AddOpenGraph(string property, string content);
    string AddJsonLd(string jsonString);
    string AddJsonLd(object jsonObject);
    string AddIcon(string path, NoParamOrder noParamOrder = default,
        string rel = "", int size = 0, string type = null);
    string AddIconSet(string path, NoParamOrder noParamOrder = default,
        object favicon = null, IEnumerable<string> rels = null, IEnumerable<int> sizes = null);
    string Activate(params string[] keys);
#pragma warning restore CS0108, CS0114

}