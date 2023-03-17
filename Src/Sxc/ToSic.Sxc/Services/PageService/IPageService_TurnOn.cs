namespace ToSic.Sxc.Services
{
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
        /// * or a object containing all the parameters which turnOn requires
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="require">
        /// _optional_ One or more requirements which must be met before the code starts.
        /// Can be one or many values and/or functions.
        /// * a `string` such as `window.myObject` or `window.myObject.readyToStart()`
        /// * a array of such strings
        /// </param>
        /// <param name="data">_optional_ any value such as a string, or an object - to pass into the run-command</param>
        /// <returns>An empty string, just so you can use it directly in Razor like `@Kit.Page.TurnOn("...")`</returns>
        /// <remarks>New in v15.</remarks>
        string TurnOn(object runOrSpecs,
            string noParamOrder = Eav.Parameters.Protector,
            object require = default,
            object data = default);

        #endregion

    }
}
