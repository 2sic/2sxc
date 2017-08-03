using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.SexyContent.Installer;
using ToSic.SexyContent.Search;
using ToSic.Eav.Apps.Interfaces;
using ToSic.SexyContent.ContentBlocks;
using DotNetNuke.Common.Utilities;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnBusinessController : ModuleSearchBase, IUpgradeable, IVersionable
    {

        #region DNN Interface Members - search, upgrade, versionable

        private IEnvironmentVersioning versioning;

        /// <summary>
        /// Constructor overload for DotNetNuke
        /// (BusinessControllerClass needs parameterless constructor)
        /// </summary>
        public DnnBusinessController()
        {
            versioning = ToSic.Eav.Factory.Resolve<IEnvironmentVersioning>();
        } //: this(0, 0) { }



        public int GetLatestVersion(int moduleId)
        {
            return versioning.GetLatestVersion(moduleId);
        }

        public int GetPublishedVersion(int moduleId)
        {
            return versioning.GetPublishedVersion(moduleId);
        }

        public void PublishVersion(int moduleId, int version)
        {
            versioning.DoInsidePublishLatestVersion(moduleId, (args) => {
                // NOTE for 2dm: Set all items of the content-block to published
            });
        }

        public void DeleteVersion(int moduleId, int version)
        {
            versioning.DoInsideDeleteLatestVersion(moduleId, (args) => {
                // NOTE for 2dm: If we want to support delete, reset any item in draft state of the content-block
            });
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