namespace ToSic.Sxc.Services;

public partial interface IPageService
{
    #region TurnOn (new v15)

    /// <summary>
    /// Turn on some javascript code when all requirements have been met.
    /// Uses [turnOn](xref:JsCode.TurnOn.Index).
    /// 
    /// Will automatically activate the feature and set hidden data on the page for the turnOn JS to pick up.
    /// </summary>
    /// <param name="runOrSpecs">
    /// * either a run `string` like `window.myObject.myJs()` (must always start with window)
    /// * or an object containing all the parameters which turnOn requires
    /// </param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="require">
    /// _optional_ One or more requirements which must be met before the code starts.
    /// Can be one or many values and/or functions.
    /// * a `string` such as `window.myObject` or `window.myObject.readyToStart()`
    /// * an array of such strings
    /// </param>
    /// <param name="data">_optional_ any value such as a string, or an object - to pass into the run-command</param>
    /// <param name="args">_optional_ array of values to pass to the run function (new v18.00)</param>
    /// <param name="condition">_optional_ condition when this should happen - if false, it won't add anything (new v16.02)</param>
    /// <param name="noDuplicates">Will not add this turnOn if an identical one is already added to the page (new 16.05)</param>
    /// <param name="addContext">
    /// _optional_ when `args` is used, the context is usually not needed or given.
    /// If you do need it, it can be merged with data (if that's also provided and is an object - so use `data`) or added to the end of the args list `end`.
    /// (new v18.00)
    /// </param>
    /// <returns>An empty string, just so you can use it directly in Razor like `@Kit.Page.TurnOn("...")`</returns>
    /// <remarks>
    /// * Added in v15.x
    /// * `condition` added in 16.02
    /// * `noDuplicates` added in 16.05
    /// * `args` and `addContext` added in v18.0
    /// </remarks>
    string TurnOn(object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object require = default,
        object data = default,
        IEnumerable<object> args = default,
        bool condition = true,
        bool? noDuplicates = default,
        string addContext = default
    );

    #endregion

}