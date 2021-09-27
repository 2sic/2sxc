using ToSic.Eav.Documentation;
#if NET451
using IHtmlString = System.Web.IHtmlString;
#else
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Conversion helper for things which are very common in web-code like Razor and WebAPIs.
    /// It's mainly a safe conversion from anything to a target-type.
    /// 
    /// Some special things it does:
    /// * Strings like "4.2" reliably get converted to int 4 which would otherwise return 0
    /// * Numbers like 42 reliably converts to bool true which would otherwise return false
    /// * Numbers like 42.5 reliably convert to strings "42.5" instead of "42,5" in certain cultures
    /// </summary>
    [PrivateApi("WIP 12.05")]
    public interface IConvertService
    {
        /// <summary>
        /// If set to true (default) will optimize converting numbers.
        /// For example, a string like "4.2" will properly convert to an int of 2.
        /// If set to false, this optimization doesn't happen and a string "4.2" would result in a 0 int
        /// </summary>
        bool OptimizeNumbers { get; }

        /// <summary>
        /// If set to true, will treat a number like 2 or -1 and strings like "2" as true.
        /// If set to false, only 1 will be true, other numbers will be false.
        /// </summary>
        bool OptimizeBoolean { get; }


        /// <summary>
        /// If set to true (default) will ensure that numbers and dates are serialized in culture-invariant mode.
        /// So numbers to strings will always use the dot "." notation and never the comma ","
        /// </summary>
        bool OptimizeRoundtrip { get; }

        /// <summary>
        /// Convert any object safely to the desired type T.
        /// If conversion fails, it will return `default(T)`, which is 0 for most numbers, `false` for boolean or `null` for strings or objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        T To<T>(object value);

        /// <summary>
        /// Convert any object safely to the desired type T.
        /// If conversion fails, it will return the `fallback` parameter as given.
        /// Since the fallback is typed, you can usually call this method without specifying T explicitly, so this should work:
        ///
        /// ```
        /// var c1 = Convert.To("5", 100); // will return 5
        /// var c2 = Convert.To("", 100);  // will return 100
        /// ```
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="fallback">The value used if conversion fails.</param>
        /// <returns></returns>
        T To<T>(object value, T fallback);


        /// <summary>
        /// Convert any object safely to int.
        /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
        /// </summary>
        int ToInt(object value);

        /// <summary>
        /// Convert any object safely to float.
        /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
        /// If you need a fallback, please use the normal <see cref="To{T}(object, T)"/> a that will work without if, the type if the fallback value has the right type.
        /// </summary>
        float ToFloat(object value);

        /// <summary>
        /// Convert any object safely to double.
        /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
        /// If you need a fallback, please use the normal <see cref="To{T}(object, T)"/> a that will work without if, the type if the fallback value has the right type.
        /// </summary>
        double ToDouble(object value);

        /// <summary>
        /// Convert any object safely to double.
        /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
        /// If you need a fallback, please use the normal <see cref="To{T}(object, T)"/> a that will work without if, the type if the fallback value has the right type.
        /// </summary>
        bool ToBool(object value);


        /// <summary>
        /// Convert any object safely to string.
        /// This does the same as <see cref="To{T}(object)"/> but this is easier to type in Razor.
        /// If you need a fallback, please use the normal <see cref="To{T}(object, T)"/> a that will work without if, the type if the fallback value has the right type.
        ///
        /// Important: this is especially useful in scenarios where a number must always have a dot "." notation, because it will ensure that even if the UI culture would use a comma ",".
        /// For example, when giving numbers to JavaScript.
        /// </summary>
        string ToString(object value);

        IHtmlString ToRaw(object value);

        /// <summary>
        /// Sub-Service to convert JSON
        /// </summary>
        IJsonService Json { get; }

        // TODO:
        // - ToDateTime
        // - ToJson
        // - JsonTo<...>
        // - JsonToObject

    }
}
