using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Context.Sys.CmsContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract class CmsContextPartBase<T>(CmsContext cmsContext, T contents) : Wrapper<T>(contents), IHasMetadata
    where T : class
{
    protected CmsContext CmsContext = cmsContext;

    /// <summary>
    /// Typed IMetadata accessor to all the metadata of this object.
    /// </summary>
    public ITypedMetadata Metadata => field ??= CmsContext.ExCtx.GetCdf().MetadataTyped(MetadataRaw);

    [JsonIgnore] // ignore, as it's published through the Metadata property which is better typed.
    IMetadata IHasMetadata.Metadata => MetadataRaw;

    private IMetadata MetadataRaw => field ??= GetMetadataOf();

    protected abstract IMetadata GetMetadataOf();
}