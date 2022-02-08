using ToSic.Eav.Context;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Code
{
    public interface IDynamicCodeService
    {
        /// <summary>
        /// Get a <see cref="IDynamicCode12"/> object for a specific Module on a page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        IDynamicCode12 OfModule(int pageId, int moduleId);

        IDynamicCode12 OfApp(int appId);
        IDynamicCode12 OfApp(int zoneId, int appId);

        IApp App(
            string noParamOrder = Eav.Parameters.Protector,
            int zoneId = Eav.Constants.IdNotInitialized,
            int appId = Eav.Constants.IdNotInitialized,
            ISite site = null,
            bool withUnpublished = false);
    }
}
