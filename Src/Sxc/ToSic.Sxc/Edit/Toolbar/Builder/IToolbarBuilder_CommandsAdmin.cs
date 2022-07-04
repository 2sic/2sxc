namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Button to admin the app. 
        /// </summary>
        /// <param name="target">_not used ATM_ just here for API consistency</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
        /// By default, the button will show based on conditions like permissions.
        /// </param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder App(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the import-app dialog
        /// </summary>
        /// <param name="target">_not used ATM_ just here for API consistency</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
        /// By default, the button will show based on conditions like permissions.
        /// </param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder AppImport(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to edit the app resources, if there are any. 
        /// </summary>
        /// <param name="target">_not used ATM_ just here for API consistency</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
        /// By default, the button will show based on conditions like permissions.
        /// </param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder AppResources(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to edit the custom app settings, if there are any. 
        /// </summary>
        /// <param name="target">_not used ATM_ just here for API consistency</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
        /// By default, the button will show based on conditions like permissions.
        /// </param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder AppSettings(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the apps management of the current site. 
        /// </summary>
        /// <param name="target">_not used ATM_ just here for API consistency</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
        /// By default, the button will show based on conditions like permissions.
        /// </param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder Apps(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the system admin dialog.
        /// </summary>
        /// <param name="target">_not used ATM_ just here for API consistency</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
        /// By default, the button will show based on conditions like permissions.
        /// </param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder System(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the insights for debugging.
        /// </summary>
        /// <param name="target">_not used ATM_ just here for API consistency</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
        /// By default, the button will show based on conditions like permissions.
        /// </param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder Insights(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

    }
}