using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Button to admin the app. 
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// By default, the button will show based on conditions like permissions.
        /// If you set `operation`, to `add` you force it to appear, with `remove` you force it to go away.
        /// </param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder App(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the import-app dialog
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// By default, the button will show based on conditions like permissions.
        /// If you set `operation`, to `add` you force it to appear, with `remove` you force it to go away.
        /// </param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder AppImport(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to edit the app resources, if there are any. 
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// By default, the button will show based on conditions like permissions.
        /// If you set `operation`, to `add` you force it to appear, with `remove` you force it to go away.
        /// </param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder AppResources(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to edit the custom app settings, if there are any. 
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// By default, the button will show based on conditions like permissions.
        /// If you set `operation`, to `add` you force it to appear, with `remove` you force it to go away.
        /// </param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder AppSettings(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the apps management of the current site. 
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// By default, the button will show based on conditions like permissions.
        /// If you set `operation`, to `add` you force it to appear, with `remove` you force it to go away.
        /// </param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder Apps(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the system admin dialog.
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// By default, the button will show based on conditions like permissions.
        /// If you set `operation`, to `add` you force it to appear, with `remove` you force it to go away.
        /// </param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder System(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the insights for debugging.
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="operation">
        /// By default, the button will show based on conditions like permissions.
        /// If you set `operation`, to `add` you force it to appear, with `remove` you force it to go away.
        /// </param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder Insights(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

    }
}