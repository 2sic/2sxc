using System;
using System.ComponentModel;
using System.Resources;

namespace ToSic.SexyContent.DataImportExport
{
    /// <summary>
    /// Attribute to describe and localize enumeration values.
    /// 
    /// Example:
    /// public enum MyEnum
    /// {
    ///     [LocalizedDescription("MyValue1Resource", typeof(MyEnum)]
    ///     MyValue1
    /// }
    /// </summary>
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