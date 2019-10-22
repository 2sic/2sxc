using ToSic.Eav.Apps.Enums;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.Edit.ClientContextInfo
{
    public class ClientInfoContentBlock
    {
        public bool ShowTemplatePicker;
        public bool IsEntity;
        public string VersioningRequirements;
        public int Id;
        public string ParentFieldName;
        public int ParentFieldSortOrder;
        public bool PartOfPage;

        internal ClientInfoContentBlock(IContentBlock contentBlock, string parentFieldName, int indexInField, PublishingMode versioningRequirements)
        {
            ShowTemplatePicker = contentBlock.ShowTemplateChooser;
            IsEntity = contentBlock.ParentIsEntity;
            Id = contentBlock.ContentBlockId;
            ParentFieldName = parentFieldName;
            ParentFieldSortOrder = indexInField;
            VersioningRequirements = versioningRequirements.ToString();
            PartOfPage = contentBlock.ParentId == contentBlock.ContentBlockId; // if the CBID is the moduleId, then it's part of page
        }
    }

}
