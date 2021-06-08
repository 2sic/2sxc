using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.WebApi.Context
{
    public interface IUiContextBuilder
    {
        /// <summary>
        /// Initialize the context builder
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        IUiContextBuilder SetZoneAndApp(int zoneId, IAppIdentity app);

        /// <summary>
        /// Get the context based on the situation
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        ContextDto Get(Ctx flags, CtxEnable enableFlags);
    }
}
