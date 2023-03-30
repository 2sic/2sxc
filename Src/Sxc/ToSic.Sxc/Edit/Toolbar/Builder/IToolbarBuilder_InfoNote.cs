using System;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Create an info, warning, help or link-button to assist the user.
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="tweak">Optional function call to tweak the button.</param>
        /// <param name="link">If provided, will make the button open the link in a new window.</param>
        /// <returns></returns>
        /// <remarks>
        /// * Added in v15.07
        /// </remarks>
        IToolbarBuilder Info(
            string noParamOrder = Eav.Parameters.Protector,
            string link = default,
            Func<ITweakButton, ITweakButton> tweak = default
        );
    }
}