using System.Collections.Immutable;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Edit.Toolbar
{
    /// <summary>
    /// Experimental new API in v15.07 to improve how to configure the Ui of a button.
    /// </summary>
    /// <remarks>
    /// Added in v15.07
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public interface ITweakButton
    {
        /// <summary>
        /// List of changes to apply to the UI parameter
        /// </summary>
        [PrivateApi] IImmutableList<object> UiMerge { get; }
        [PrivateApi] IImmutableList<object> ParamsMerge { get; }

        #region UI

        /// <summary>
        /// Add a floating note to the button.
        /// </summary>
        /// <param name="note">The note/message</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="type">Optional type, like `info` (default), `warning`, `help`</param>
        /// <returns></returns>
        ITweakButton Note(
            string note = default,
            string noParamOrder = Eav.Parameters.Protector,
            string type = default
        );

        /// <summary>
        /// Set the show of this button.
        /// </summary>
        /// <param name="show">Optional show value, default is `true`</param>
        /// <returns></returns>
        ITweakButton Show(bool show = true);

        /// <summary>
        /// Set the color of this button.
        /// </summary>
        /// <param name="color">The main color parameter. Can contain two values, comma separated.</param>
        /// <param name="noParamOrder"></param>
        /// <param name="background">Background color - will only take affect if the `color` was not set.</param>
        /// <param name="foreground">Foreground color - will only take affect if the `color` was not set.</param>
        /// <returns></returns>
        ITweakButton Color(
            string color = default,
            string noParamOrder = Eav.Parameters.Protector,
            string background = default,
            string foreground = default
        );

        /// <summary>
        /// Set the title / Tooltip of the button.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        ITweakButton Tooltip(string title);

        /// <summary>
        /// Set what group the button is in.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        ITweakButton Group(string group);

        /// <summary>
        /// Set the icon for this button.
        /// </summary>
        /// <param name="icon">One of a few predefined names, or a SVG string.</param>
        /// <returns></returns>
        ITweakButton Icon(string icon);

        /// <summary>
        /// Set one or more classes on the button.
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        ITweakButton Class(string classes);

        /// <summary>
        /// Add a general ui-rule for things which are not in the standard API.
        /// </summary>
        /// <param name="ui">a string such as `this=that` or an object which will be parsed/serialized such as `new { this = 27 }`</param>
        /// <returns></returns>
        ITweakButton Ui(object ui);

        /// <summary>
        /// Add a general UI rule for things which are not in the standard API.
        /// </summary>
        /// <param name="name">The name/key which comes before the `=`</param>
        /// <param name="value">The value which comes after the `=`</param>
        /// <returns></returns>
        ITweakButton Ui(string name, string value);

        #endregion

        #region Params

        ITweakButton Parameters(object value);
        ITweakButton Parameters(string name, string value);

        ITweakButton Prefill(object prefill);

        ITweakButton Filter(object filter);

        #endregion
    }
}
