using ToSic.Eav.Apps.State;
using ToSic.Eav.Context;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Context;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CmsSite: CmsContextPartBase<ISite>, ICmsSite
{
    public ICmsSite Init(CmsContext parent, IAppStateInternal appState)
    {
        base.Init(parent, parent.CtxSite.Site);
        _appState = appState;
        return this;
    }

    private IAppStateInternal _appState;

    public int Id => GetContents()?.Id ?? Eav.Constants.NullId;
    public string Url => GetContents()?.Url ?? string.Empty;
    public string UrlRoot => GetContents().UrlRoot ?? string.Empty;

    protected override IMetadataOf GetMetadataOf() 
        => ExtendWithRecommendations(_appState.GetMetadataOf(TargetTypes.Site, Id, Url));
}