using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ClientInfoContentBlock
    {
        public string VersioningRequirements;
        public int Id;
        public string ParentFieldName;
        public int ParentFieldSortOrder;
        public bool PartOfPage;

        internal ClientInfoContentBlock(IBlock contentBlock, string parentFieldName, int indexInField, PublishingMode versioningRequirements)
        {
            Id = contentBlock.ContentBlockId;
            ParentFieldName = parentFieldName;
            ParentFieldSortOrder = indexInField;
            VersioningRequirements = versioningRequirements.ToString();

            // if the CBID is the Mod-Id, then it's part of page
            PartOfPage = contentBlock.ParentId == contentBlock.ContentBlockId;
        }
    }

}
