using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Code.Context.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeContext<out TModel, out TServiceKit>: ICodeContext, IHasKit<TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit;