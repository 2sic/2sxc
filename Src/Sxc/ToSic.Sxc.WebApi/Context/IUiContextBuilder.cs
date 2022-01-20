using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.WebApi.Context
{
    public interface IUiContextBuilder
    {
        /// <summary>
        /// Initialize the context builder
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        IUiContextBuilder InitApp(IAppIdentity app);

        /// <summary>
        /// Get the context based on the situation
        /// </summary>
        /// <returns></returns>
        ContextDto Get(Ctx flags, CtxEnable enableFlags);
    }
}
