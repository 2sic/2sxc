using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Button to change the view/layout of the data shown on the page. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Layout(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to run JS code. 
        /// </summary>
        /// <param name="target">Name of the function to call, without parameters. </param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Code(
            object target,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open a dialog to manage the fields/attributes of the content type. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Fields(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the edit-template (source-code) dialog. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Template(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the design/edit query dialog. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Query(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to open the edit view settings dialog. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder View(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );




    }
}