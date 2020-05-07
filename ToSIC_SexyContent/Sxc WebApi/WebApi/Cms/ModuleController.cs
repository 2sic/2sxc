using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ItemListActions;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Data;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Security;
using Assembly = System.Reflection.Assembly;
using Factory = ToSic.Eav.Factory;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SxcApiController
    {
        protected CmsRuntime CmsRuntime => _cmsRuntime ?? (_cmsRuntime = GetCmsRuntime());
        private CmsRuntime _cmsRuntime;

        protected CmsManager CmsManager => _cmsManager ?? (_cmsManager = new CmsManager(App, Log));
        private CmsManager _cmsManager;

        private CmsRuntime GetCmsRuntime()
            // todo: this must be changed, set showDrafts to true for now, as it's probably only used in the view-picker, but it shoudln't just be here
            => App == null ? null : new CmsRuntime(App, Log, true, false);


        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sModC");
            BlockEditor = BlockBuilder.Block.Editor;
        }

        private BlockEditorBase BlockEditor { get; set;  }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void AddItem([FromUri] int? sortOrder = null)
        {
            Log.Add($"add order:{sortOrder}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args) =>
                CmsManager.Blocks.AddEmptyItem(BlockBuilder.Block.Configuration, sortOrder);

            // use dnn versioning - this is always part of page
            versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            var permCheck = new MultiPermissionsApp(BlockBuilder, App.AppId, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, out var exp))
                throw exp;

            return BlockEditor.SaveTemplateId(templateId, forceCreateContentGroup);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetAppId(int? appId) => BlockEditor.SetAppId(appId);

        #region Get Apps, ContentTypes and Views for UI

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<AppUiInfo> GetSelectableApps(string apps = null)
        {
            // we must get the zone-id from the environment,
            // since the app may not yet exist when inserted the first time
            var tenant = new DnnTenant(PortalSettings.Current);
            var tenantZoneId = Env.ZoneMapper.GetZoneId(tenant);
            var list = new CmsZones(tenantZoneId, Env, Log).AppsRt.GetSelectableApps(tenant).ToList();

            if (string.IsNullOrWhiteSpace(apps)) return list;

            // New feature in 10.27 - if app-list is provided, only return these
            var appNames = apps.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            list = list.Where(ap => appNames
                    .Any(name => string.Equals(name, ap.Name, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
            return list;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeUiInfo> GetSelectableContentTypes() =>
            CmsRuntime?.Views.GetContentTypesWithStatus();


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<TemplateUiInfo> GetSelectableTemplates() => CmsRuntime?.Views.GetCompatibleViews(App, BlockBuilder.Block.Configuration);

        #endregion

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public string GenerateContentBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
        {
            Log.Add($"get CB parent:{parentId}, field:{field}, order:{sortOrder}, app:{app}, guid:{guid}");
            var contentTypeName = Settings.AttributeSetStaticNameContentBlockTypeName;
            var values = new Dictionary<string, object>
            {
                {BlockFromEntity.CbPropertyTitle, ""},
                {BlockFromEntity.CbPropertyApp, app},
                {BlockFromEntity.CbPropertyShowChooser, true},
            };
            var newGuid = guid ?? Guid.NewGuid();
            var entityId = CreateItemAndAddToList(parentId, field, sortOrder, contentTypeName, values, newGuid);

            // now return a rendered instance
            var newContentBlock = new BlockFromEntity(BlockBuilder.Block, entityId, Log);
            return newContentBlock.BlockBuilder.Render().ToString();

        }

        private int CreateItemAndAddToList(int parentId, string field, int sortOrder, string contentTypeName,
            Dictionary<string, object> values, Guid newGuid)
        {
            var cgApp = BlockBuilder.App;

            // create the new entity 
            var entityId = new AppManager(cgApp, Log).Entities.GetOrCreate(newGuid, contentTypeName, values);

            #region attach to the current list of items

            var cbEnt = BlockBuilder.App.Data.List.One(parentId);
            var blockList = ((IEnumerable<IEntity>) cbEnt.GetBestValue(field))?.ToList() ?? new List<IEntity>();

            var intList = blockList.Select(b => b.EntityId).ToList();
            // add only if it's not already in the list (could happen if http requests are run again)
            if (!intList.Contains(entityId))
            {
                if (sortOrder > intList.Count) sortOrder = intList.Count;
                intList.Insert(sortOrder, entityId);
            }
            var updateDic = new Dictionary<string, object> {{field, intList}};
            new AppManager(cgApp, Log).Entities.UpdateParts(cbEnt.EntityId, updateDic);
            #endregion
            
            return entityId;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void MoveItemInList(int parentId, string field, int indexFrom, int indexTo, [FromUri] bool partOfPage = false)
        {
            Log.Add($"move item in list parent:{parentId}, field:{field}, from:{indexFrom}, to:{indexTo}, partOfpage:{partOfPage}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args) => new AppManager(BlockBuilder.App, Log)
                .Entities.ModifyItemList(parentId, field, new Move(indexFrom, indexTo));

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
            else InternalSave(null);
        }

        /// <summary>
        /// Delete a content-block inside a list of content-blocks
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="field"></param>
        /// <param name="index"></param>
        /// <param name="partOfPage"></param>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveItemInList(int parentId, string field, int index, [FromUri] bool partOfPage = false)
        {
            Log.Add($"remove item: parent{parentId}, field:{field}, index:{index}, partOfPage{partOfPage}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args) => new AppManager(BlockBuilder.App, Log)
                .Entities.ModifyItemList(parentId, field, new Remove(index));

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
            else InternalSave(null);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage RenderTemplate([FromUri] int templateId, [FromUri] string lang, bool cbIsEntity = false)
        {
            Log.Add($"render template:{templateId}, lang:{lang}, isEnt:{cbIsEntity}");
            try
            {
                // Try setting thread language to enable 2sxc to render the template in this language
                if (!string.IsNullOrEmpty(lang))
                    try
                    {
                        var culture = global::System.Globalization.CultureInfo.GetCultureInfo(lang);
                        global::System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    }
                    // Fallback / ignore if the language specified has not been found
                    catch (global::System.Globalization.CultureNotFoundException) { }

                var cbToRender = BlockBuilder.Block;

                // if a real templateId was specified, swap to that
                if (templateId > 0)
                {
                    var template = new CmsRuntime(cbToRender.App, Log, Edit.Enabled, false).Views.Get(templateId);
                    ((BlockBuilder)cbToRender.BlockBuilder).View = template;
                }

                var rendered = cbToRender.BlockBuilder.Render().ToString();

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rendered, Encoding.UTF8, "text/plain")
                };

            }
            catch (Exception e)
            {
				Exceptions.LogException(e);
                throw;
            }
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void ChangeOrder([FromUri] int sortOrder, int destinationSortOrder)
        {
            Log.Add($"change order sort:{sortOrder}, dest:{destinationSortOrder}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args) =>
                CmsManager.Blocks.ChangeOrder(BlockBuilder.Block.Configuration, sortOrder, destinationSortOrder);

            // use dnn versioning - items here are always part of list
            versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int sortOrder)
        {
            Log.Add($"try to publish #{sortOrder} on '{part}'");
            if (!new MultiPermissionsApp(BlockBuilder, App.AppId, Log)
                .EnsureAll(GrantSets.WritePublished, out var exp))
                throw exp;
            return BlockEditor.Publish(part, sortOrder);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public bool Publish(int id)
        {
            Log.Add($"try to publish id #{id}");
            if (!new MultiPermissionsApp(BlockBuilder, App.AppId, Log)
                .EnsureAll(GrantSets.WritePublished, out var exp))
                throw exp;
            new AppManager(App, Log).Entities.Publish(id);
            return true;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveFromList(int sortOrder, Guid? parent, string fields)
        {
            var wrapLog = Log.Call($"remove from index:{sortOrder}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args)
            {
                var target = parent == null ? BlockBuilder.Block.Configuration.Entity : App.Data.List.One(parent.Value);
                if (target == null) throw new Exception($"Can't find parent {parent}");
                var useVersioning = BlockBuilder.Block.Configuration.VersioningEnabled;
                var fieldList = fields?.Split(',').Select(f => f.Trim()).ToArray() ?? ViewParts.ContentPair;
                CmsManager.Entities.FieldListRemove(target, fieldList, sortOrder, useVersioning);
            }

            // use dnn versioning - items here are always part of list
            versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
            wrapLog(null);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public string RemoteInstallDialogUrl(string dialog, bool isContentApp)
        {
            // note / warning: some duplicate code with SystemController.cs
            // ReSharper disable StringLiteralTypo
            
            if (dialog != "gettingstarted")
                throw new Exception("unknown dialog name: " + dialog);


            var moduleInfo = Request.FindModuleInfo();
            var modName = moduleInfo.DesktopModule.ModuleName;

            // new: check if it should allow this
            // it should only be allowed, if the current situation is either
            // Content - and no views exist (even invisible ones)
            // App - and no apps exist - this is already checked on client side, so I won't include a check here
            if (isContentApp)
            {
                // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                try
                {
                    var all = new CmsRuntime(BlockBuilder.App, Log, Edit.Enabled, false).Views.GetAll();
                    if (all.Any())
                        return null;
                }
                catch
                {
                    // ignored
                }
            }

            // Add desired destination
            // Add DNN Version, 2SexyContent Version, module type, module id, Portal ID
            var gettingStartedSrc = "//gettingstarted.2sxc.org/router.aspx?"
                                    + "destination=autoconfigure" + (isContentApp ? Eav.Constants.ContentAppName.ToLower() : "app")
                + "&DnnVersion=" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)
                + "&2SexyContentVersion=" + Settings.ModuleVersion
                + "&ModuleName=" + modName + "&ModuleId=" + moduleInfo.ModuleID
                + "&PortalID=" + moduleInfo.PortalID;
            // Add VDB / Zone ID (if set)
            var zoneId = Env.ZoneMapper.GetZoneId(moduleInfo.PortalID);
            gettingStartedSrc +=  "&ZoneID=" + zoneId;
                                    // ReSharper restore StringLiteralTypo

            // Add DNN Guid
            var hostSettings = HostController.Instance.GetSettingsDictionary();
            gettingStartedSrc += hostSettings.ContainsKey("GUID") ? "&DnnGUID=" + hostSettings["GUID"] : "";
            // Add Portal Default Language & current language
            gettingStartedSrc += "&DefaultLanguage=" + PortalSettings.DefaultLanguage
                + "&CurrentLanguage=" + PortalSettings.CultureCode;

            // Set src to iframe
            return gettingStartedSrc;
        }

        /// <summary>
        /// Finish system installation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool FinishInstallation()
        {
            Log.Add("finish installation");
            var ic = Factory.Resolve<IEnvironmentInstaller>();
            if (ic.IsUpgradeRunning)
                throw new Exception("There seems to be an upgrade running - please wait. If you still see this message after 10 minutes, please restart the web application.");

            ic.ResumeAbortedUpgrade();

            return true;
        }

    }
}