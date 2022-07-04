using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        // TODO: FORGOT OPERATION ON THESE COMMANDS

        /// <summary>
        /// Button to add an entity to a list of entities
        /// </summary>
        /// <param name="target">
        /// _optional_ entity-like target which is in a list of items in on a content-block,
        /// see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="contentType"></param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder Add(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to add an existing entity to the list. 
        /// </summary>
        /// <param name="target">
        /// _optional_ entity-like target which is in a list of items in on a content-block,
        /// see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="contentType"></param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder AddExisting(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to manage the list of entities shown here. 
        /// </summary>
        /// <param name="target">
        /// _optional_ entity-like target which is in a list of items in on a content-block,
        /// see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder List(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            //string contentType = null,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to move an item down in a list
        /// </summary>
        /// <param name="target">
        /// _optional_ entity-like target which is in a list of items in on a content-block,
        /// see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder MoveDown(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to move an item up in a list. 
        /// </summary>
        /// <param name="target">
        /// _optional_ entity-like target which is in a list of items in on a content-block,
        /// see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder MoveUp(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to remove an item from a list.
        /// This will not delete the item, just remove. 
        /// </summary>
        /// <param name="target">
        /// _optional_ entity-like target which is in a list of items in on a content-block,
        /// see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder Remove(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to replace the current item in the list with another existing item. 
        /// </summary>
        /// <param name="target">
        /// _optional_ entity-like target which is in a list of items in on a content-block,
        /// see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
        /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        IToolbarBuilder Replace(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null
        );
    }
}