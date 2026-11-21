using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Ui.Edit.Sys;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Update the identifier with information if it will affect a ContentBlock.
/// </summary>
/// <remarks>
/// This is important for later processing, as it may change how certain things behave.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditIdentifierProviderContentGroupModeDetection()
    : ServiceWithSetup<IAppWorkContext>("Api.GrpPrc"),
        IEditLoadIdentifierProvider
{
    public const int WorkSequenceOrderDefault = 100; // quite early, so that other providers can use the information
    public int WorkSequenceOrder => WorkSequenceOrderDefault; // quite early

    public Task<Package<List<ItemIdentifier>>> Handle(WorkContext mainCtx, Package<List<ItemIdentifier>> package)
    {
        var appCtx = mainCtx.TryGet<IAppWorkContext>(EditLoadContextConstants.AppWorkCtx)
            ?? MyOptions;
        return Task.FromResult(package.RePackage(ConvertGroup(appCtx, package.Data)));
    }


    private List<ItemIdentifier> ConvertGroup(IAppWorkContext appCtx, List<ItemIdentifier> identifiers)
    {
        var l = Log.Fn<List<ItemIdentifier>>();
        var result = identifiers
            .Select(ItemIdentifier? (identifier) => identifier == null! /* paranoid */
                ? null
                : identifier with { IsContentBlockMode = DetectContentBlockMode(appCtx, identifier) }
            )
            .OfType<ItemIdentifier>()
            .ToList();
        return l.Return(result);
    }


    /// <summary>
    /// Check if the save will affect a ContentBlock.
    /// If it's a simple entity-edit or edit of item inside a normal field list, it returns false
    /// </summary>
    /// <returns></returns>
    private bool DetectContentBlockMode(IAppWorkContext appCtx, ItemIdentifier identifier)
    {
        var l = Log.Fn<bool>();
        if (!identifier.Parent.HasValue)
            return l.ReturnFalse("no parent");

        // get the entity and determine if it's a content-block. If yes, that should affect the differences in load/save
        var entity = appCtx.AppReader.List.GetOne(identifier.Parent.Value)!;
        var isContentGroup = entity.Type.Name == AppConstants.ContentGroupTypeName;
        return l.Return(isContentGroup, $"type name should match: {isContentGroup}");
    }
}
