using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsSite(CmsContext parent, IAppReader appReader)
    : CmsContextPartBase<ISite>(parent, parent.CtxSite.Site), ICmsSite
{
    public int Id => GetContents()?.Id ?? Eav.Constants.NullId;
    public string Url => GetContents()?.Url ?? string.Empty;
    public string UrlRoot => GetContents().UrlRoot ?? string.Empty;

    protected override IMetadataOf GetMetadataOf() 
        => appReader.Metadata.GetMetadataOf(TargetTypes.Site, Id, title: Url)
            .AddRecommendations();
}