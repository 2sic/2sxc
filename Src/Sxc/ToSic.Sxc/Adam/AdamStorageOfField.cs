using System;
using ToSic.Eav.Apps.Adam;
using ToSic.Eav.Identity;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Container of the assets of a field
    /// each entity+field combination has its own container for assets
    /// </summary>
    public class AdamStorageOfField<TFolderId, TFileId>: AdamStorage<TFolderId, TFileId>
    {
        private readonly Guid _entityGuid;
        private readonly string _fieldName;

        public AdamStorageOfField(AdamManager<TFolderId, TFileId> manager, Guid eGuid, string fName) : base(manager)
        {
            _entityGuid = eGuid;
            _fieldName = fName;
        }


        protected override string GeneratePath(string subFolder)
        {
            var callLog = Log.Call<string>(subFolder);
            var result = AdamConstants.ItemFolderMask
                .Replace("[AdamRoot]", Manager.Path)
                .Replace("[Guid22]", Mapper.GuidCompress(_entityGuid))
                .Replace("[FieldName]", _fieldName)
                .Replace("[SubFolder]", subFolder) // often blank, so it will just be removed
                .Replace("//", "/");
            return callLog(result, result);
        }
    }
}