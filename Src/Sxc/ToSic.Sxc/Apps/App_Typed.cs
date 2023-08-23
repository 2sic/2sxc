using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Data;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Decorators;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Apps.AppAssetFolderMain;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public partial class App: IAppTyped
    {
        #region IAppTyped Folder / Thumbnail (replaces Paths)

        IFolder IAppTyped.Folder => _folder ?? (_folder = (this as IAppTyped).FolderAdvanced());
        private IFolder _folder;

        IFolder IAppTyped.FolderAdvanced(string noParamOrder, string location)
        {
            Protect(noParamOrder, nameof(location));
            return new AppAssetFolderMain(this, DetermineShared(location) ?? AppState.IsShared());
        }

        IFile IAppTyped.Thumbnail => _thumbnailFile.Get(() => new AppAssetThumbnail(this, _globalPaths));
        private readonly GetOnce<IFile> _thumbnailFile = new GetOnce<IFile>();

        #endregion

        /// <inheritdoc cref="IAppTyped.Settings"/>
        ITypedItem IAppTyped.Settings => AppSettings == null ? null : _typedSettings.Get(() => MakeTyped(AppSettings, propsRequired: true));
        private readonly GetOnce<ITypedItem> _typedSettings = new GetOnce<ITypedItem>();

        /// <inheritdoc cref="IAppTyped.Resources"/>
        ITypedItem IAppTyped.Resources => _typedRes.Get(() => MakeTyped(AppResources, propsRequired: true));
        private readonly GetOnce<ITypedItem> _typedRes = new GetOnce<ITypedItem>();

        private ITypedItem MakeTyped(IEntity contents, bool propsRequired)
        {
            var wrapped = CmsEditDecorator.Wrap(contents, false);
            return _cdfLazy.Value.AsItem(wrapped, Protector, propsRequired: propsRequired);
        }

    }
}
