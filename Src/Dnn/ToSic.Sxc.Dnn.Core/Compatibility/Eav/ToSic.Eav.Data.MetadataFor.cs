// 2019-11-13 2dm - introduced this, because we moved metadatafor to Metadata (backwards compatibility)
// it's used by others, like on https://stackoverflow.com/questions/55814403/2sxc-web-api-create-content-item-metadata-for-adam-asset

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Data
{
    public class MetadataFor: Metadata.Target
    {
    }
}
