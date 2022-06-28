using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
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
        /// <param name="operation"></param>
        /// <returns></returns>
        IToolbarBuilder Code(
            object target,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        IToolbarBuilder Fields(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        IToolbarBuilder Template(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        IToolbarBuilder Query(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        IToolbarBuilder View(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );




    }
}