using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Dnn;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Dnn.Services
{
    [PrivateApi]
    public class DnnPageChanges : HasLog<DnnPageChanges> // , IPageChangeApplicator
    {
        //public IPageService PageService { get; }
        public PageServiceShared PageServiceShared { get; }

        public DnnPageChanges(/*IPageService pageChanges,*/ PageServiceShared pageServiceShared): base($"{DnnConstants.LogName}.PgeCng")
        {
            //PageService = pageChanges;
            PageServiceShared = pageServiceShared;
        }


        public int Apply()
        {
            var wrapLog = Log.Call<int>();
            // If we get something invalid, return 0 (nothing changed)
            // if (!(PageServiceShared != null /*is IChangeQueue changes*/)) return wrapLog(null, 0);

            var dnnPage = new DnnHtmlPage();
            var props = PageServiceShared.GetPropertyChangesAndFlush();
            foreach (var p in props)
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

            var count = props.Count;

            //// Once processed clean up, in case the same object (scoped) is used again, and we want to ensure it won't be processed again
            //foreach (var p in changes.PropertyChanges.ToArray()) // ToArray important to "copy" so we can do removes afterwards
            //    changes.PropertyChanges.Remove(p);

            // Note: we're not implementing replace etc. in DNN
            // ATM there's no reason to, maybe some other time
            var headChanges = PageServiceShared.GetHeadChangesAndFlush();
            foreach (var h in headChanges)
                dnnPage.AddToHead(h.Tag);

            count += headChanges.Count;

            // Clean up
            //foreach (var h in changes.Headers.ToArray())
            //    changes.Headers.Remove(h);

            return wrapLog($"{count}", count);
        }


    }
}
