using System;
using System.Collections.Generic;
using Oqtane.Models;

namespace ToSic.Sxc.Oqt.Shared.Models;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SxcResource: Resource
{
    public string UniqueId { get; set; }

    ///// <summary>
    ///// For the contents of a script tag
    ///// </summary>
    //public string Content { get; set; }

    public bool IsExternal { get; set; } = true;

    /// <summary>
    /// Used to store all other html attributes from html tag.
    /// </summary>
    /// <remarks>
    /// Setting HtmlAttributes will also try to set Integrity and CrossOrigin properties
    /// that are explicitly supported by Oqtane.Models.Resource.
    /// </remarks>
    public IDictionary<string, string> HtmlAttributes
    {
        get => _htmlAttributes;
        set
        {
            // copy dictionary or null;
            _htmlAttributes = value == null ? null : new Dictionary<string, string>(value, StringComparer.InvariantCultureIgnoreCase);
                
            // set additional html attribute properties that are explicitly supported by Oqtane.Models.Resource
            HtmlAttributesGetValueAndRemoveKey("integrity", value => Integrity = value);
            HtmlAttributesGetValueAndRemoveKey("crossorigin", value => CrossOrigin = value);
        }
    }
    private IDictionary<string, string> _htmlAttributes = null;

    /// <summary>
    /// Helper method used to set key value on static key-property,
    /// and remove key from HtmlAttributes dictionary
    /// </summary>
    /// <param name="key"></param>
    /// <param name="setter">property setter</param>
    private void HtmlAttributesGetValueAndRemoveKey(string key, Action<string> setter)
    {
        if (HtmlAttributes == null) return;
        if (!HtmlAttributes.ContainsKey(key)) return;
        if (string.IsNullOrEmpty(HtmlAttributes[key])) return;

        // set new value only when we have it
        setter(HtmlAttributes[key]);
        HtmlAttributes.Remove(key);
    }

    public new string Url
    {
        get => base.Url;
        set
        {
            // fix issue in Oqtane 3.2
            // can't set Resource.Url to null
            if (value != null) base.Url = value;
        }
    }

    /// <summary>
    /// The type of the resource, e.g. "module" for ES6 modules
    /// </summary>
    /// <remarks>
    /// Added for compatibility with Oqtane 6.0.0 or older.
    /// Remove when Oqtane dependency is updated to 6.0.1
    /// </remarks>
    public string Type { get; set; }

    /// <summary>
    /// Old property used to determine if the resource is an ES6 module before Oqtane 6.0.1 where is replayed with Type property.
    /// </summary>
    /// <remarks>
    /// Added for compatibility with Oqtane 6.0.0 or older.
    /// Remove when Oqtane dependency is updated to 6.0.1
    /// </remarks>
    public bool ES6Module
    {
        get => (Type == "module");
        set
        {
            if (value)
            {
                Type = "module";
            };
        }
    }
}