using System;
using ToSic.Eav.Identity;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Container of the assets of a field
    /// each entity+field combination has its own container for assets
    /// </summary>
    public class AdamOfField<TFolderId, TFileId>: AdamOfBase<TFolderId, TFileId>
    {
        private readonly Guid _entityGuid;
        private readonly string _fieldName;

        public AdamOfField(AdamManager<TFolderId, TFileId> manager, Guid eGuid, string fName) : base(manager)
        {
            _entityGuid = eGuid;
            _fieldName = fName;
        }


        protected override string GeneratePath(string subFolder)
            => Configuration.ItemFolderMask
                .Replace("[AdamRoot]", Manager.Path)
                .Replace("[Guid22]", Mapper.GuidCompress(_entityGuid))
                .Replace("[FieldName]", _fieldName)
                .Replace("[SubFolder]", subFolder) // often blank, so it will just be removed
                .Replace("//", "/"); // sometimes has duplicate slashes if subfolder blank but sub-sub is given

    }
}