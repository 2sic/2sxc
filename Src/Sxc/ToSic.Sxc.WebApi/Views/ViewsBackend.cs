using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Views
{
    public class ViewsBackend: HasLog
    {
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly IAppStates _appStates;
        private readonly ISite _site;
        private readonly IUser _user;

        public ViewsBackend(Lazy<CmsManager> cmsManagerLazy, IContextOfSite context, IAppStates appStates) : base("Bck.Views")
        {
            _cmsManagerLazy = cmsManagerLazy;
            _appStates = appStates;

            _site = context.Site;
            _user = context.User;
        }

        public ViewsBackend Init(ILog parentLog)
        {
            Log.LinkTo(parentLog);
            return this;
        }


        public IEnumerable<ViewDetailsDto> GetAll(int appId)
        {
            Log.Add($"get all a#{appId}");
            var cms = _cmsManagerLazy.Value.Init(_appStates.Identity(null, appId), true, Log).Read;

            var attributeSetList = cms.ContentTypes.All.OfScope(Settings.AttributeSetScope).ToList();
            var templateList = cms.Views.GetAll().ToList();
            Log.Add($"attribute list count:{attributeSetList.Count}, template count:{templateList.Count}");
            var templates = templateList.Select(c => new ViewDetailsDto
            {
                Id = c.Id, Name = c.Name, ContentType = TypeSpecs(attributeSetList, c.ContentType, c.ContentItem),
                PresentationType = TypeSpecs(attributeSetList, c.PresentationType, c.PresentationItem),
                ListContentType = TypeSpecs(attributeSetList, c.HeaderType, c.HeaderItem),
                ListPresentationType = TypeSpecs(attributeSetList, c.HeaderPresentationType, c.HeaderPresentationItem),
                TemplatePath = c.Path,
                IsHidden = c.IsHidden,
                ViewNameInUrl = c.UrlIdentifier,
                Guid = c.Guid,
                List = c.UseForList,
                HasQuery = c.QueryRaw != null,
                Used = c.Entity.Parents().Count,
                IsShared = c.IsShared,
                Permissions = new HasPermissionsDto {Count = c.Entity.Metadata.Permissions.Count()},
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
        private static ViewContentTypeDto TypeSpecs(IEnumerable<IContentType> allCTs, string staticName, IEntity maybeEntity)
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
            var app = _cmsManagerLazy.Value.ServiceProvider.Build<ImpExpHelpers>().Init(Log).GetAppAndCheckZoneSwitchPermissions(_site.ZoneId, appId, _user, _site.ZoneId);
            var cms = _cmsManagerLazy.Value.Init(app, Log);
            cms.Views.DeleteView(id);
            return true;
        }
    }
}
