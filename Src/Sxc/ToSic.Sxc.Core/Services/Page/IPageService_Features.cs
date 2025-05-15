namespace ToSic.Sxc.Services;

public partial interface IPageService
{

    #region Features

    /// <summary>
    /// Activate a feature on this page, such as `turnOn`, `2sxc.JsCore` etc.
    /// For list of features, see [](xref:NetCode.Razor.Services.IPageServiceActivate).
    /// </summary>
    /// <param name="keys">One or more strings containing Page-Feature keys</param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.Activate(...)`</returns>
    string Activate(params string[] keys);

    /// <summary>
    /// Activate a feature on this page, such as `turnOn`, `2sxc.JsCore` etc.
    /// For list of features, see [](xref:NetCode.Razor.Services.IPageServiceActivate).
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="condition">Condition to determine if activation should happen</param>
    /// <param name="features">One or more strings containing Page-Feature keys</param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.Activate(...)`</returns>
    /// <remarks>
    /// * This overload with `condition` added in v15.03
    /// </remarks>
    string Activate(
        NoParamOrder noParamOrder = default,
        bool condition = true,
        params string[] features);

    #endregion


}