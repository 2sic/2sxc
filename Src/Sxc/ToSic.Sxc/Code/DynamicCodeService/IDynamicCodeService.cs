using ToSic.Eav.Context;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Code
{
    public interface IDynamicCodeService
    {
        IDynamicCode12 OfModule(int pageId, int moduleId);

        //IDynamicCode OfApp(int appId);

        IApp App(
            string noParamOrder = Eav.Parameters.Protector,
            int zoneId = Eav.Constants.IdNotInitialized,
            int appId = Eav.Constants.IdNotInitialized,
            ISite site = null,
            bool withUnpublished = false);
    }
}
