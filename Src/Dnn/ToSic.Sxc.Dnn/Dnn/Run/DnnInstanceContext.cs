using System;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Run.Context;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnContextOfBlock: ContextOfBlock
    {
        private readonly IPagePublishingResolver _publishingResolver;

        public DnnContextOfBlock(
            IServiceProvider serviceProvider,
            ISite site, 
            IUser user, 
            IContainer container, 
            IPage page, 
            IPagePublishingResolver publishingResolver) 
            : base(serviceProvider, site, user)
        {
            _publishingResolver = publishingResolver;
            Container = container;
            Page = page;
        }

        public override BlockPublishingState Publishing => _publishing ?? (_publishing = _publishingResolver.GetPublishingState(Container.Id));
        private BlockPublishingState _publishing;

        public DnnContextOfBlock Init(ModuleInfo dnnModule, ILog parentLog)
        {
            ((DnnSite)Site).TrySwap(dnnModule);
            ((DnnContainer)Container).Init(dnnModule, parentLog);
            return this;
        }

        //public DnnContextOfBlock Init(/*IContainer container,*/ List<KeyValuePair<string, string>> overrideParams = null)
        //{
        //    Page.Parameters = overrideParams;
        //    //var page = BuildPage(Site, ServiceProvider, overrideParams);
        //    //if (swapSite != null) Site = swapSite;

        //    //base.Init(page, Container, _publishingResolver.GetPublishingState(Container.Id));
        //    return this;
        //}

        //public static IContextOfBlock Create(ISite site, IContainer container, IServiceProvider serviceProvider, List<KeyValuePair<string, string>> overrideParams = null) 
        //{
        //    var page = BuildPage(site, serviceProvider, overrideParams);

        //    var publishing = serviceProvider.Build<IPagePublishingResolver>();

        //    var newContext = serviceProvider.Build<IContextOfBlock>();
        //    newContext.Site = site;

        //    return newContext // new ContextOfBlock(site, user, serviceProvider)
        //        .Init(page, container, publishing.GetPublishingState(container.Id));
        //}

        //private static SxcPage BuildPage(ISite site, IServiceProvider serviceProvider, List<KeyValuePair<string, string>> overrideParams)
        //{
        //    // Collect / assemble page information
        //    var activeTab = (site as Site<PortalSettings>)?.UnwrappedContents?.ActiveTab;
        //    // the FullUrl will throw an error in search scenarios
        //    string fullUrl = null;
        //    try
        //    {
        //        fullUrl = activeTab?.FullUrl;
        //    }
        //    catch
        //    {
        //        /* ignore */
        //    }

        //    //overrideParams = FindBestUrlParams(serviceProvider, overrideParams);
        //    var page = serviceProvider.Build<SxcPage>().Init(activeTab?.TabID ?? Eav.Constants.NullId, fullUrl, overrideParams);
        //    return page;
        //}

        //private static List<KeyValuePair<string, string>> FindBestUrlParams(IServiceProvider serviceProvider, List<KeyValuePair<string, string>> overrideParams)
        //{
        //    overrideParams = overrideParams
        //                     ?? serviceProvider.Build<IHttp>()?.QueryStringKeyValuePairs()
        //                     ?? new List<KeyValuePair<string, string>>();
        //    return overrideParams;
        //}
    }
}
