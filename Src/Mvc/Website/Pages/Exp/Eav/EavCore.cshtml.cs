using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Run;

namespace Website.Pages
{
    public class EavCoreModel : PageModel
    {
        public DataSourceFactory DataSourceFactory { get; }
        protected const int ZoneId = 2;
        protected const int AppId = 4;

        public EavCoreModel(DataSourceFactory dataSourceFactory)
        {
            DataSourceFactory = dataSourceFactory;
        }


        public void OnGet()
        {
            EntityInBlog = State.Get(AppId).List.First();
        }

        public IEntity EntityInBlog;

        private ILookUpEngine ConfigProvider => new LookUpEngine(null);

        public IDataSource BlogRoot()
        {
            return DataSourceFactory.GetDataSource<IAppRoot>(new AppIdentity(ZoneId, AppId), null, ConfigProvider);
        }

        public IDataSource BlogTags()
        {
            var dsBlog = BlogRoot();
            var dsFilter = DataSourceFactory.GetDataSource<EntityTypeFilter>(dsBlog, dsBlog);
            dsFilter.TypeName = "Tag";
            return dsFilter;
        }

        public IApp BlogApp => _blogApp ??= ToSic.Sxc.Mvc.Factory.App(ZoneId, AppId, DataSourceFactory.ServiceProvider.Build<ISite>().Init(TestIds.PrimaryZone), false, null);
        private IApp _blogApp;
    }
}