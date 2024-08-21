using ToSic.Eav.Apps;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsSite: CmsContextPartBase<ISite>, ICmsSite
{
    public ICmsSite Init(CmsContext parent, IAppReader appState)
    {
        base.Init(parent, parent.CtxSite.Site);
        _appState = appState;
        return this;
    }

    private IAppReader _appState;

    public int Id => GetContents()?.Id ?? Eav.Constants.NullId;
    public string Url => GetContents()?.Url ?? string.Empty;
    public string UrlRoot => GetContents().UrlRoot ?? string.Empty;

    protected override IMetadataOf GetMetadataOf() 
        => ExtendWithRecommendations(_appState.GetMetadataOf(TargetTypes.Site, Id, Url));
}