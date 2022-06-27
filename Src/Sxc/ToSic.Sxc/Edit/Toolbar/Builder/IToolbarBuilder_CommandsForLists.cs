using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {

        IToolbarBuilder Add(
            object target = null,
            string noParamOrder = Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null
        );

        IToolbarBuilder AddExisting(
            object target = null,
            string noParamOrder = Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null
        );

        IToolbarBuilder List(
            object target = null,
            string noParamOrder = Parameters.Protector,
            //string contentType = null,
            object ui = null,
            object parameters = null
        );

        IToolbarBuilder MoveDown(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        IToolbarBuilder MoveUp(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        IToolbarBuilder Remove(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        );

        IToolbarBuilder Replace(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        );
    }
}