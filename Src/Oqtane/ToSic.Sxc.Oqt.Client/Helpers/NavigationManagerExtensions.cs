using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using UrlHelpers = ToSic.Sxc.Oqt.Shared.UrlHelpers;

namespace ToSic.Sxc.Oqt.Client;

/// <summary>
/// https://chrissainty.com/working-with-query-strings-in-blazor/
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class NavigationManagerExtensions
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T value)
    {
        var uri = navManager.ToAbsoluteUri(navManager.Uri);

        if (TryGetValue(UrlHelpers.ParseQueryString(uri.Query), key, out var valueFromQueryString))
        {
            if (typeof(T) == typeof(bool) && bool.TryParse(valueFromQueryString, out var valueAsBool))
            {
                value = (T)(object)valueAsBool;
                return true;
            }

            if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out var valueAsInt))
            {
                value = (T)(object)valueAsInt;
                return true;
            }

            if (typeof(T) == typeof(string))
            {
                value = (T)(object)valueFromQueryString.ToString();
                return true;
            }

            if (typeof(T) == typeof(decimal) && decimal.TryParse(valueFromQueryString, out var valueAsDecimal))
            {
                value = (T)(object)valueAsDecimal;
                return true;
            }
        }

        value = default;
        return false;
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static bool TryGetValue(NameValueCollection collection, string key, out string value)
    {
        value = collection[key];
        return value != null;
    }

}