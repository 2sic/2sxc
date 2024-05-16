namespace ToSic.Sxc.Edit.Toolbar;

public partial interface IToolbarBuilder
{
    /// <summary>
    /// Create button to **admin the app**.
    /// Can also be used to remove the same button on a toolbar which would have it by default. 
    /// </summary>
    /// <param name="target">_not used ATM_ just here for API consistency</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="operation">
    /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
    /// By default, the button will show based on conditions like permissions.
    /// </param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder App(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );

    /// <summary>
    /// Create button to open the **import-app** dialog.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">_not used ATM_ just here for API consistency</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="operation">
    /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
    /// By default, the button will show based on conditions like permissions.
    /// </param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder AppImport(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );

    /// <summary>
    /// Create button to **edit the app resources** if there are any.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">_not used ATM_ just here for API consistency</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="operation">
    /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
    /// By default, the button will show based on conditions like permissions.
    /// </param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder AppResources(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );

    /// <summary>
    /// Create button to **edit the custom app settings** if there are any.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">_not used ATM_ just here for API consistency</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="operation">
    /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
    /// By default, the button will show based on conditions like permissions.
    /// </param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder AppSettings(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );

    /// <summary>
    /// Create button to open the **apps management** of the current site.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">_not used ATM_ just here for API consistency</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="operation">
    /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
    /// By default, the button will show based on conditions like permissions.
    /// </param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder Apps(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );

    /// <summary>
    /// Create button to open the **system admin** dialog.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">_not used ATM_ just here for API consistency</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="operation">
    /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
    /// By default, the button will show based on conditions like permissions.
    /// </param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder System(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );

    /// <summary>
    /// Create button to open the **insights** for debugging.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">_not used ATM_ just here for API consistency</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="operation">
    /// _optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation).
    /// By default, the button will show based on conditions like permissions.
    /// </param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder Insights(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );

}