using ToSic.Eav.Data;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Decorators;

namespace ToSic.Sxc.Apps
{
    public partial class App: IAppTyped
    {
        /// <inheritdoc cref="IAppTyped.Settings"/>
        ITypedItem IAppTyped.Settings => AppSettings == null ? null : _typedSettings.Get(() => MakeTyped(AppSettings, strict: true));
        private readonly GetOnce<ITypedItem> _typedSettings = new GetOnce<ITypedItem>();

        /// <inheritdoc cref="IAppTyped.Resources"/>
        ITypedItem IAppTyped.Resources => _typedRes.Get(() => MakeTyped(AppResources, strict: true));
        private readonly GetOnce<ITypedItem> _typedRes = new GetOnce<ITypedItem>();

        private ITypedItem MakeTyped(IEntity contents, bool strict)
        {
            var wrapped = CmsEditDecorator.Wrap(contents, false);
            return _cdfLazy.Value.AsItem(wrapped, Eav.Parameters.Protector, strict: strict);
        }

    }
}
