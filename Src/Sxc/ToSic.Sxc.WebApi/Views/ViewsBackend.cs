using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Views
{
    internal class ViewsBackend: HasLog
    {
        private ITenant _tenant;
        private IUser _user;

        public ViewsBackend() : base("Bck.Views") { }

        public ViewsBackend Init(ITenant tenant, IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _tenant = tenant;
            _user = user;
            return this;
        }


        public IEnumerable<ViewDetailsDto> GetAll(int appId)
        {
            Log.Add($"get all a#{appId}");
            var cms = new CmsRuntime(appId, Log, true);

            var attributeSetList = cms.ContentTypes.FromScope(Settings.AttributeSetScope).ToList();
            var templateList = cms.Views.GetAll().ToList();
            Log.Add($"attribute list count:{attributeSetList.Count}, template count:{templateList.Count}");
            var templates = templateList.Select(c => new ViewDetailsDto
            {
                Id = c.Id, Name = c.Name, ContentType = MiniCTSpecs(attributeSetList, c.ContentType, c.ContentItem),
                PresentationType = MiniCTSpecs(attributeSetList, c.PresentationType, c.PresentationItem),
                ListContentType = MiniCTSpecs(attributeSetList, c.HeaderType, c.HeaderItem),
                ListPresentationType = MiniCTSpecs(attributeSetList, c.HeaderPresentationType, c.HeaderPresentationItem),
                TemplatePath = c.Path,
                IsHidden = c.IsHidden,
                ViewNameInUrl = c.UrlIdentifier,
                Guid = c.Guid,
                List = c.UseForList,
                HasQuery = c.QueryRaw != null,
                Used = c.Entity.Parents().Count
            });
            return templates;
        }


        /// <summary>
        /// Helper to prepare a quick-info about 1 content type
        /// </summary>
        /// <param name="allCTs"></param>
        /// <param name="staticName"></param>
        /// <param name="maybeEntity"></param>
        /// <returns></returns>
        private ViewContentTypeDto MiniCTSpecs(IEnumerable<IContentType> allCTs, string staticName, IEntity maybeEntity)
        {
            var found = allCTs.FirstOrDefault(ct => ct.StaticName == staticName);
            return new ViewContentTypeDto
            {
                StaticName = staticName, Id = found?.ContentTypeId ?? 0, Name = found == null ? "no content type" : found.Name,
                DemoId = maybeEntity?.EntityId ?? 0,
                DemoTitle = maybeEntity?.GetBestTitle() ?? ""
            };
        }

        public bool Delete(int appId, int id)
        {
            // todo: extra security to only allow zone change if host user
            Log.Add($"delete a{appId}, t:{id}");
            var app = ImpExpHelpers.GetAppAndCheckZoneSwitchPermissions(_tenant.ZoneId, appId, _user, _tenant.ZoneId, Log);
            var cms = new CmsManager(app, Log);
            cms.Views.DeleteTemplate(id);
            return true;
        }
    }
}
