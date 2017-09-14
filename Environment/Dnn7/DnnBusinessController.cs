using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.SexyContent.Installer;
using ToSic.SexyContent.Search;
using ToSic.Eav.Apps.Interfaces;
using ToSic.SexyContent.ContentBlocks;
using DotNetNuke.Common.Utilities;
using ToSic.Eav.Apps;
using System.Linq;
using ToSic.Eav.Interfaces;
using ToSic.SexyContent.EAVExtensions;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnBusinessController : ModuleSearchBase, IUpgradeable, IVersionable
    {
        #region DNN Interface Members - search, upgrade, versionable

        private IPagePublishing versioning;

        /// <summary>
        /// Constructor overload for DotNetNuke
        /// (BusinessController needs a parameterless constructor)
        /// </summary>
        public DnnBusinessController()
        {
            versioning = new PagePublishing();
        }

        public int GetLatestVersion(int moduleId) => versioning.GetLatestVersion(moduleId);

        public int GetPublishedVersion(int moduleId) => versioning.GetPublishedVersion(moduleId);

        public void PublishVersion(int moduleId, int version)
        {
            try
            {
                // publish all entites of this content block
                var moduleInfo = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
                var cb = new ModuleContentBlock(moduleInfo);
                var appManager = new AppManager(cb.AppId);

                // Add content entities
                var list = cb.Data["Default"]?.LightList ?? new List<IEntity>();

                // Add list content if defined
                var cont = cb.Data.Out.ContainsKey("ListContent") ? cb.Data["ListContent"]?.LightList : null;
                if (cont != null) list = list.Concat(cont);

                // Find related presentation entities
                var presentationItems = list
                    .Where(e => (e as EntityInContentGroup)?.Presentation != null)
                    .Select(e => ((EntityInContentGroup) e).Presentation);
                list = list.Concat(presentationItems);

                var ids = list.Where(e => !e.IsPublished).Select(e => e.EntityId).ToList();

                // publish ContentGroup as well - if there already is one
                if (cb.ContentGroup != null)
                    ids.Add(cb.ContentGroup.ContentGroupId);

                if (ids.Any())
                    appManager.Entities.Publish(ids.ToArray());

                // Set published version
                // 2017-09-13 2dm - not sure if this is needed, may cause trouble because then DNN says the page changed again 
                // + isn't necessary, because DNN will publish the settings
                var moduleVersionSettings = new PagePublishing.ModuleVersionSettingsController(moduleId);
                moduleVersionSettings.PublishLatestVersion();
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                throw;
            }
        }

        public void DeleteVersion(int moduleId, int version)
        {
            //versioning.DoInsideDeleteLatestVersion(moduleId, (args) => {
            //    // NOTE for 2dm: If we want to support delete, reset any item in draft state of the content-block
            //});
        }

        public int RollBackVersion(int moduleId, int version)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This is part of the IUpgradeable of DNN
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public string UpgradeModule(string version)
        {
            return new InstallationController().UpgradeModule(version);
        }

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            try
            {
                return new SearchController().GetModifiedSearchDocuments(moduleInfo, beginDate);
            }
            catch (Exception e)
            {
                throw new SearchIndexException(moduleInfo, e);
            }
        }

        #endregion
    }
}