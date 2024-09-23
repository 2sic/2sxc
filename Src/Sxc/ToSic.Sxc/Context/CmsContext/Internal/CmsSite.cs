using ToSic.Eav.Apps;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsSite: CmsContextPartBase<ISite>, ICmsSite
{
    public ICmsSite Init(CmsContext parent, IAppReader appReader)
    {
        base.Init(parent, parent.CtxSite.Site);
        _appReader = appReader;
        return this;
    }

    private IAppReader _appReader;

    public int Id => GetContents()?.Id ?? Eav.Constants.NullId;
    public string Url => GetContents()?.Url ?? string.Empty;
    public string UrlRoot => GetContents().UrlRoot ?? string.Empty;

    protected override IMetadataOf GetMetadataOf() 
        => ExtendWithRecommendations(_appReader.Metadata.GetMetadataOf(TargetTypes.Site, Id, title: Url));
}