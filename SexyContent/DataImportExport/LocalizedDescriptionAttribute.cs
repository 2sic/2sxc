using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    // TODO2tk: Move helper classes (not data import specific) to a common folder
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string resourceKey;
        
        private readonly ResourceManager resourceManager;
        
        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            this.resourceManager = new ResourceManager(resourceType.FullName, resourceType.Assembly);
            this.resourceKey = resourceKey;
        }

        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType, string resourceFolder)
        {
            this.resourceManager = new ResourceManager(resourceFolder + "." + resourceType.Name, resourceType.Assembly);
            this.resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string displayName = resourceManager.GetString(resourceKey);
                if (string.IsNullOrEmpty(displayName))
                {
                    return string.Format("[{0}]", resourceKey);
                }
                return displayName;
            }
        }
    }
}