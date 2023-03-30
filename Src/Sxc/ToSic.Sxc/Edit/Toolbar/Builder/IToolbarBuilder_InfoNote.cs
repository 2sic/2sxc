using System;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Edit.Toolbar
{
    [PublicApi]
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Create an info, warning, help or link-button to assist the user.
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="link">If provided, will make the button open the link in a new window.</param>
        /// <param name="tweak">Optional function call to tweak the button.</param>
        /// <returns></returns>
        [PrivateApi("WIP v15.04")]
        IToolbarBuilder Info(
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            string link = default
        );
    }
}