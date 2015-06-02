using System;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Implementations.ValueConverter;

namespace ToSic.Eav.ImportExport.Refactoring.ValueConverter
{
    public class SexyContentVC : IEavValueConverter
    {
        private string hlnkType = "Hyperlink";

        //public bool UseOnCachePopulation = false;
        //public bool UseOnValueRead = true;
        //public bool UseOnValueCreate = false;
        //public bool UseOnValueSave = true;

        // public bool AllowSingletonPerApp = true;

        public string Convert(ConversionScenario scenario, string type, string originalValue)
        {
            switch (scenario)
            {
                case ConversionScenario.GetFriendlyValue:
                    if (type == hlnkType)
                        throw new NotImplementedException();
                    break;
                case ConversionScenario.ConvertFriendlyToData:
                    if (type == hlnkType)
                        return TryToResolveOneLinkToInternalDnnCode(originalValue);
                    break;
            }

            return originalValue;
        }

        /// <summary>
        /// Will take a link like http:\\... to a file or page and try to return a DNN-style info like
        /// Page:35 or File:43003
        /// </summary>
        /// <param name="potentialFilePath"></param>
        /// <returns></returns>
        public string TryToResolveOneLinkToInternalDnnCode(string potentialFilePath)
        {
            var portalInfo = PortalController.Instance.GetCurrentPortalSettings();

            // Try file reference
            var fileInfo = FileManager.Instance.GetFile(portalInfo.PortalId, potentialFilePath);
            if (fileInfo != null)
                return "File:" + fileInfo.FileId;

            // Try page / tab ID
            var tabController = new TabController();
            var tabCollection = tabController.GetTabsByPortal(portalInfo.PortalId);
            var tabInfo = tabCollection.Select(tab => tab.Value)
                                       .Where(tab => tab.TabPath == potentialFilePath)
                                       .FirstOrDefault();
            if (tabInfo != null)
                return "Page:" + tabInfo.TabID;

            return potentialFilePath;
        }


    }




}