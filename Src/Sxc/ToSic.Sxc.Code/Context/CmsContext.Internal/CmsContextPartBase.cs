using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Lib.Data;
using ToSic.Lib.Helpers;
using ToSic.Lib.Wrappers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Context.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract class CmsContextPartBase<T>(CmsContext parent, T contents) : Wrapper<T>(contents), IHasMetadata
    where T : class
{
    protected CmsContext Parent = parent;

    /// <summary>
    /// Typed IMetadata accessor to all the metadata of this object.
    /// </summary>
    public IMetadata Metadata => _dynMeta.Get(() => Parent.ExCtx.GetCdf().Metadata(MetadataRaw));
    private readonly GetOnce<IMetadata> _dynMeta = new();

    [JsonIgnore] // ignore, as it's published through the Metadata property which is better typed.
    IMetadataOf IHasMetadata.Metadata => MetadataRaw;

    private IMetadataOf MetadataRaw => _md.Get(GetMetadataOf);
    private readonly GetOnce<IMetadataOf> _md = new();

    protected abstract IMetadataOf GetMetadataOf();
}