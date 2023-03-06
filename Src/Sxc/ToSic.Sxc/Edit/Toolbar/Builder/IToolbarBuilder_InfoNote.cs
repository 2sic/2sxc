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
        /// <param name="note">A string containing the note, or a object with the note and more specs.</param>
        /// <param name="link">If provided, will make the button open the link in a new window.</param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
        /// <returns></returns>
        [PrivateApi("WIP v15.04")]
        IToolbarBuilder Info(
            string noParamOrder = Eav.Parameters.Protector,
            object note = default,
            string link = default,
            object ui = default,
            object parameters = default,
            string operation = default
        );
    }
}