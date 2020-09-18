using System;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamItemDto
    {
        public bool IsFolder, AllowEdit;
        public int Size, MetadataId;
        public string Path, Name, Type;
        public DateTime Created, Modified;

        public AdamItemDto(bool isFolder, string name, int size, DateTime created, DateTime modified)
        {
            IsFolder = isFolder;
            // note that the type will be set by other code later on if it's a file
            Type = isFolder ? "folder" : "unknown";
            Name = name;
            Size = size;
            Created = created;
            Modified = modified;
        }

    }
}
