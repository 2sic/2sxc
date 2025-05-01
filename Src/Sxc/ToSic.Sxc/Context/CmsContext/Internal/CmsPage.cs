﻿using ToSic.Eav.Apps;
using ToSic.Eav.Metadata;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsPage(CmsContext parent, IMetadataOfSource appState, LazySvc<IPage> fallbackPage)
    : CmsContextPartBase<IPage>(parent, parent?.CtxBlockOrNull?.Page ?? fallbackPage.Value), ICmsPage
{
    public int Id => GetContents()?.Id ?? 0;
    public IParameters Parameters => GetContents()?.Parameters;
    public string Url => GetContents().Url ?? string.Empty;

    protected override IMetadataOf GetMetadataOf() =>
        ExtendWithRecommendations(appState.GetMetadataOf(TargetTypes.Page, Id, title: Url),
            [Decorators.NoteDecoratorName, Decorators.OpenGraphName]);
}