using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.TestStuff;
using ToSic.Sxc.Web;

namespace Website.Pages
{
    public class EavCoreModel : PageModel
    {
        protected const int ZoneId = 2;
        protected const int AppId = 78;

        public EavCoreModel()
        {
        }


        public void OnGet()
        {
            EntityInBlog = State.Get(AppId).List.First();
        }

        public IEntity EntityInBlog;

        private ILookUpEngine ConfigProvider => new LookUpEngine(null);

        public IDataSource BlogRoot()
        {
            var dsFact = new DataSource();
            return dsFact.GetDataSource<IAppRoot>(new AppIdentity(ZoneId, AppId), null, ConfigProvider);
        }

        public IDataSource BlogTags()
        {
            var dsFact = new DataSource();
            var dsBlog = BlogRoot();
            var dsFilter = dsFact.GetDataSource<EntityTypeFilter>(dsBlog, dsBlog);
            dsFilter.TypeName = "Tag";
            return dsFilter;
        }

        public IApp BlogApp => _blogApp ??= ToSic.Sxc.Mvc.Factory.App(ZoneId, AppId, new MvcTenant(new MvcPortalSettings()), false, false, null);
        private IApp _blogApp;
    }
}