using System;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamItemDto<TFolderId, TItemId>: AdamItemDto
    {
        public TItemId Id;
        public TFolderId ParentId;

        public AdamItemDto(bool isFolder, TItemId id, TFolderId parentId, string name, int size, DateTime created, DateTime modified)
            :base(isFolder, name, size, created, modified)
        {
            Id = id;
            ParentId = parentId;
            //IsFolder = isFolder;
            //// note that the type will be set by other code later on if it's a file
            //Type = isFolder ? "folder" : "unknown";
            //Name = name;
            //Size = size;
            //Created = created;
            //Modified = modified;
        }

    }
}
