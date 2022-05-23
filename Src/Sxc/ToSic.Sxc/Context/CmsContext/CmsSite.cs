using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("Hide implementation")]
    public class CmsSite: CmsContextPartBase<ISite>, ICmsSite
    {
        public CmsSite(Lazy<App> siteAppLazy) => _siteAppLazy = siteAppLazy;
        private readonly Lazy<App> _siteAppLazy;

        public ICmsSite Init(CmsContext parent, AppState appState)
        {
            base.Init(parent, parent.CtxSite.Site);
            _appState = appState;
            return this;
        }

        private AppState _appState;

        public int Id => _contents?.Id ?? Eav.Constants.NullId;
        public string Url => _contents?.Url ?? string.Empty;
        public string UrlRoot => _contents.UrlRoot ?? string.Empty;

        public IApp App => _app.Get(() => _siteAppLazy.Value.Init(_appState, null, null));
        private readonly ValueGetOnce<IApp> _app = new ValueGetOnce<IApp>();

        protected override IMetadataOf GetMetadataOf() 
            => _appState.GetMetadataOf(TargetTypes.Site, Id, Url);
    }
}
