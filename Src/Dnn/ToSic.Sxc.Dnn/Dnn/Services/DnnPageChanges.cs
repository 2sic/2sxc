using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Dnn;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Dnn.Services
{
    [PrivateApi]
    public class DnnPageChanges : HasLog<DnnPageChanges>
    {
        public PageServiceShared PageServiceShared { get; }

        public DnnPageChanges(PageServiceShared pageServiceShared): base($"{DnnConstants.LogName}.PgeCng")
        {
            PageServiceShared = pageServiceShared;
        }


        public int Apply()
        {
            var wrapLog = Log.Call<int>();

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

            // Note: we're not implementing replace etc. in DNN
            // ATM there's no reason to, maybe some other time
            var headChanges = PageServiceShared.GetHeadChangesAndFlush();
            foreach (var h in headChanges)
                dnnPage.AddToHead(h.Tag);

            // New in 12.04 - Add features which have HTML only
            // Todo STV: Repeat this in Oqtane
            // In the page the code would be like this:
            // var pageService = GetService<ToSic.Sxc.Web.IPageService>();
            // pageService.Activate("fancybox4");
            // This will add a header for the sources of these features
            PageServiceShared.Features
                .ManualFeaturesGetNew()
                .ForEach(f => dnnPage.AddToHead(Tag.Custom(f.Html)));

            count += headChanges.Count;

            return wrapLog($"{count}", count);
        }


    }
}
