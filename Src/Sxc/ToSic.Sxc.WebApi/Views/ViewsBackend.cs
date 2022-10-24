using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Serialization;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.ImportExport;


namespace ToSic.Sxc.WebApi.Views
{
    public class ViewsBackend: HasLog
    {
        public ViewsBackend(Lazy<CmsManager> cmsManagerLazy, IContextOfSite context, IAppStates appStates, Lazy<IConvertToEavLight> convertToEavLight, GeneratorLog<ImpExpHelpers> impExpHelpers) : base("Bck.Views")
        {
            _cmsManagerLazy = cmsManagerLazy;
            _appStates = appStates;
            _convertToEavLight = convertToEavLight;
            _impExpHelpers = impExpHelpers.SetLog(Log);

            _site = context.Site;
            _user = context.User;
        }

        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly IAppStates _appStates;
        private readonly Lazy<IConvertToEavLight> _convertToEavLight;
        private readonly GeneratorLog<ImpExpHelpers> _impExpHelpers;
        private readonly ISite _site;
        private readonly IUser _user;

        public ViewsBackend Init(ILog parentLog)
        {
            Log.LinkTo(parentLog);
            return this;
        }


        public IEnumerable<ViewDetailsDto> GetAll(int appId)
        {
            Log.A($"get all a#{appId}");
            var cms = _cmsManagerLazy.Value.Init(_appStates.IdentityOfApp(appId), true, Log).Read;

            var attributeSetList = cms.ContentTypes.All.OfScope(Scopes.Default /*Settings.AttributeSetScope*/).ToList();
            var viewList = cms.Views.GetAll().ToList();
            Log.A($"attribute list count:{attributeSetList.Count}, template count:{viewList.Count}");
            var ser = (_convertToEavLight.Value as ConvertToEavLight);
            var views = viewList.Select(view => new ViewDetailsDto
            {
                Id = view.Id, Name = view.Name, ContentType = TypeSpecs(attributeSetList, view.ContentType, view.ContentItem),
                PresentationType = TypeSpecs(attributeSetList, view.PresentationType, view.PresentationItem),
                ListContentType = TypeSpecs(attributeSetList, view.HeaderType, view.HeaderItem),
                ListPresentationType = TypeSpecs(attributeSetList, view.HeaderPresentationType, view.HeaderPresentationItem),
                TemplatePath = view.Path,
                IsHidden = view.IsHidden,
                ViewNameInUrl = view.UrlIdentifier,
                Guid = view.Guid,
                List = view.UseForList,
                HasQuery = view.QueryRaw != null,
                Used = view.Entity.Parents().Count,
                IsShared = view.IsShared,
                EditInfo = new EditInfoDto(view.Entity),
                Metadata = ser?.CreateListOfSubEntities(view.Metadata, SubEntitySerialization.AllTrue()),
                Permissions = new HasPermissionsDto {Count = view.Entity.Metadata.Permissions.Count()},
            });
            return views;
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
            var found = allCTs.FirstOrDefault(ct => ct.NameId == staticName);
            return new ViewContentTypeDto
            {
                StaticName = staticName, Id = found?.Id ?? 0, Name = found == null ? "no content type" : found.Name,
                DemoId = maybeEntity?.EntityId ?? 0,
                DemoTitle = maybeEntity?.GetBestTitle() ?? ""
            };
        }

        public bool Delete(int appId, int id)
        {
            // todo: extra security to only allow zone change if host user
            Log.A($"delete a{appId}, t:{id}");
            var app = _impExpHelpers.New.GetAppAndCheckZoneSwitchPermissions(_site.ZoneId, appId, _user, _site.ZoneId);
            var cms = _cmsManagerLazy.Value.Init(app, Log);
            cms.Views.DeleteView(id);
            return true;
        }
    }
}
