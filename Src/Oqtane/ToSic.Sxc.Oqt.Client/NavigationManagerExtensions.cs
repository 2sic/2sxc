using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace ToSic.Sxc.Oqt.Client
{
    /// <summary>
    /// https://chrissainty.com/working-with-query-strings-in-blazor/
    /// </summary>
    public static class NavigationManagerExtensions
    {
        public static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T value)
        {
            var uri = navManager.ToAbsoluteUri(navManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
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
    }
}
