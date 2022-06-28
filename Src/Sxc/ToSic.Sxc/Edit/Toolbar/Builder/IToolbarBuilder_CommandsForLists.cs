using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Button to add an entity to a list of entities
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="contentType"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder Add(
            object target = null,
            string noParamOrder = Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to add an existing entity to the list. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="contentType"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder AddExisting(
            object target = null,
            string noParamOrder = Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to manage the list of entities shown here. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder List(
            object target = null,
            string noParamOrder = Parameters.Protector,
            //string contentType = null,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to move an item down in a list
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder MoveDown(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to move an item up in a list. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder MoveUp(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to remove an item from a list.
        /// This will not delete the item, just remove. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder Remove(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Button to replace the current item in the list with another existing item. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder Replace(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        );
    }
}