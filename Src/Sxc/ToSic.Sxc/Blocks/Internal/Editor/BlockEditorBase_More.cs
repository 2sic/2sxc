namespace ToSic.Sxc.Blocks.Internal;

public partial class BlockEditorBase
{
    // methods which the entity-implementation must customize - so it's virtual

    protected abstract void SavePreviewTemplateId(Guid templateGuid);

    internal abstract void SetAppId(int? appId);

    internal abstract void EnsureLinkToContentGroup(Guid cgGuid);

    internal abstract void UpdateTitle(IEntity titleItem);

}