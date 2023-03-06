using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Edit.Toolbar
{
    [PublicApi]
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="noParamOrder"></param>
        /// <param name="note"></param>
        /// <param name="link">If provided, will make the button open the link in a new window.</param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation"></param>
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