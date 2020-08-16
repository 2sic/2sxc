using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ClientInfoContentBlock
    {
        // 2020-08-14 #2146 2dm believe unused
        //public bool ShowTemplatePicker;
        // 2020-08-16 clean-up #2148
        //public bool IsEntity;
        public string VersioningRequirements;
        public int Id;
        public string ParentFieldName;
        public int ParentFieldSortOrder;
        public bool PartOfPage;

        internal ClientInfoContentBlock(IBlock contentBlock, string parentFieldName, int indexInField, PublishingMode versioningRequirements)
        {
            // 2020-08-14 #2146 2dm believe unused
            //ShowTemplatePicker = contentBlock.ShowTemplateChooser;
            // 2020-08-16 clean-up #2148
            //IsEntity = contentBlock.ParentIsEntity;
            Id = contentBlock.ContentBlockId;
            ParentFieldName = parentFieldName;
            ParentFieldSortOrder = indexInField;
            VersioningRequirements = versioningRequirements.ToString();
            PartOfPage = contentBlock.ParentId == contentBlock.ContentBlockId; // if the CBID is the moduleId, then it's part of page
        }
    }

}
