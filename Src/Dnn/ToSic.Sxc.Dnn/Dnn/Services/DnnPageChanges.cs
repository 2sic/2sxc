using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Razor.Dnn;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Dnn.Services
{
    [PrivateApi]
    public class DnnPageChanges : IPageChangeApplicator
    {
        public IPageService PageService { get; }

        public DnnPageChanges(IPageService pageChanges)
        {
            PageService = pageChanges;
        }


        public int Apply()
        {
            // If we get something invalid, return 0 (nothing changed)
            if (!(PageService is IChangeQueue changes)) return 0;

            var dnnPage = new DnnHtmlPage();

            foreach (var p in changes.PropertyChanges)
                switch (p.Property)
                {
                    case PageProperties.Base:
                        dnnPage.AddBase(p.Value);
                        break;
                    case PageProperties.Title:
                        dnnPage.Title = Helpers.UpdateProperty(dnnPage.Title, p);
                        break;
                    case PageProperties.Description:
                        dnnPage.Description = Helpers.UpdateProperty(dnnPage.Description, p);
                        break;
                    case PageProperties.Keywords:
                        dnnPage.Keywords = Helpers.UpdateProperty(dnnPage.Keywords, p);
                        break;
                        //default:
                        //    throw new ArgumentOutOfRangeException();
                }

            var count = changes.PropertyChanges.Count;

            // Once processed clean up, in case the same object (scoped) is used again, and we want to ensure it won't be processed again
            foreach (var p in changes.PropertyChanges.ToArray()) // ToArray important to "copy" so we can do removes afterwards
                changes.PropertyChanges.Remove(p);

            // Note: we're not implementing replace etc. in DNN
            // ATM there's no reason to, maybe some other time
            foreach (var h in changes.Headers)
                dnnPage.AddToHead(h.Tag);

            count += changes.Headers.Count;

            // Clean up
            foreach (var h in changes.Headers.ToArray())
                changes.Headers.Remove(h);

            return count;
        }

    }
}
