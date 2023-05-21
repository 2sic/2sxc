using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is minor cross-concerns aspect of Dynamic-Entity-like objects
    /// </summary>
    [PrivateApi]
    public partial interface IDynamicEntityBase: ICanDebug
    {
        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
        ///// <summary>
        ///// Activate debugging, so that you'll see details in [Insights](xref:NetCode.Debug.Insights.Index) how the value was retrieved.
        ///// </summary>
        ///// <param name="debug"></param>
        //void SetDebug(bool debug);



    }
}
