using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Apps
{
    public partial class App: IAppTyped
    {
        /// <inheritdoc cref="IAppTyped.Settings"/>
        ITypedItem IAppTyped.Settings => AppSettings == null ? null : _typedSettings.Get(() => MakeDynProperty(AppSettings, strict: true));
        private readonly GetOnce<ITypedItem> _typedSettings = new GetOnce<ITypedItem>();

        /// <inheritdoc cref="IAppTyped.Resources"/>
        ITypedItem IAppTyped.Resources => _typedRes.Get(() => MakeDynProperty(AppResources, strict: true));
        private readonly GetOnce<ITypedItem> _typedRes = new GetOnce<ITypedItem>();
    }
}
