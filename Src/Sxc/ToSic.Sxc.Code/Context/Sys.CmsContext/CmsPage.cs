using ToSic.Eav.Metadata;
using ToSic.Eav.Metadata.Sys;

namespace ToSic.Sxc.Context.Sys.CmsContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CmsPage(CmsContext parent, IMetadataOfSource appState, LazySvc<IPage> fallbackPage)
    : CmsContextPartBase<IPage>(parent, parent.CtxBlockOrNull?.Page ?? fallbackPage.Value), ICmsPage
{
    public int Id => GetContents()?.Id ?? 0;

    // note: made it non-nullable v20 2025-06-20 2dm, not sure if it's even relevant...
    [field: AllowNull, MaybeNull] public IParameters Parameters => field ??= GetContents()!.Parameters;

    public string Url => GetContents()!.Url ?? string.Empty;

    protected override IMetadata GetMetadataOf() =>
        appState.GetMetadataOf(TargetTypes.Page, Id, title: Url)
            .AddRecommendations([KnownDecorators.NoteDecoratorName, KnownDecorators.OpenGraphName]);
}