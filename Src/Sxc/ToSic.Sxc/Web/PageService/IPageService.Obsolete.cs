using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Razor.Markup;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Web
{
    // Important: There is a critical bug in Razor that methods which an interface inherits
    // Will fail when called using dynamic parameters. 
    // https://stackoverflow.com/questions/3071634/strange-behaviour-when-using-dynamic-types-as-method-parameters
    // Because of this,
    // - ToSic.Sxc.Web.IPageService.SetTitle("ok") works
    // - ToSic.Sxc.Web.IPageService.SetTitle(dynEntity.Title) fails!!!
    // This is why each method on the underlying interface must be repeated here :(
    //
    // We suggest that we won't do this for new commands, but all commands that were in 12.08 must be repeated here

    /// <summary>
    /// Old name for the IPageService, it's in use in some v12 App templates so we must keep it working.
    /// Will continue to work, but shouldn't be used. Please use <see cref="ToSic.Sxc.Services.IPageService"/>  instead
    /// </summary>
    [Obsolete("Use ToSic.Sxc.Services.IPageService instead")]
    public interface IPageService: ToSic.Sxc.Services.IPageService
    {
        // This repeats the definition on the IPage Service
        // For reasons we cannot explain, Razor sometimes otherwise complains
        // that a GetService<ToSic.Sxc.Web.IPageService>()
        // Doesn't contain this command
        // We don't know why - once this is added here everything works
        // So for now we just leave it in

#pragma warning disable CS0108, CS0114
        [PrivateApi] void SetBase(string url = null);
        [PrivateApi] void SetTitle(string value, string placeholder = null);
        [PrivateApi] void SetDescription(string value, string placeholder = null);
        [PrivateApi] void SetKeywords(string value, string placeholder = null);
        [PrivateApi] void SetHttpStatus(int statusCode, string message = null);
        [PrivateApi] void AddToHead(string tag);
        [PrivateApi] void AddToHead(TagBase tag);
        [PrivateApi] void AddMeta(string name, string content);
        [PrivateApi] void AddOpenGraph(string property, string content);
        [PrivateApi] void AddJsonLd(string jsonString);
        [PrivateApi] void AddJsonLd(object jsonObject);
        [PrivateApi] void AddIcon(string path, string doNotRelyOnParameterOrder = Eav.Parameters.Protector, 
            string rel = "", int size = 0, string type = null);
        [PrivateApi] void AddIconSet(string path, string doNotRelyOnParameterOrder = Eav.Parameters.Protector,
            object favicon = null, IEnumerable<string> rels = null, IEnumerable<int> sizes = null);
        [PrivateApi] void Activate(params string[] keys);
#pragma warning restore CS0108, CS0114

    }
}
