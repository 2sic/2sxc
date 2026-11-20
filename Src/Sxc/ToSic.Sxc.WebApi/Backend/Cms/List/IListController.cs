using ToSic.Eav.WebApi.Admin.Sys;

namespace ToSic.Sxc.Backend.Cms;

public interface IListController: IAppAdminWebApiController
{
    /// <summary>
    /// used to be GET Module/ChangeOrder
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="fields"></param>
    /// <param name="index"></param>
    /// <param name="toIndex"></param>
    // UI test: Not called by eav-ui; Manage Content List reordering posts the complete list to Items instead.
    void Move(Guid? parent, string fields, int index, int toIndex);

    /// <summary>
    /// Used to be Get Module/RemoveFromList
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="part"></param>
    /// <param name="index"></param>
    // UI test: Manage Content List > remove an item from the list.
    void Delete(Guid? parent, string part, int index);

    // UI test: Replace Content dialog > select an item, or add an item to the list.
    void Replace(Guid parent, string part, int index, int entityId, bool add = false);
    // UI test: Open Replace Content; loading the dialog retrieves the available replacements.
    ListSlotSwapInfo ReplaceOptions(Guid parent, string part, int index, string? contentType = null);

    // UI test: Open Manage Content List; loading the dialog retrieves the current items.
    List<ListSlotContentsInfo> Items(Guid parent, string part);
    // UI test: Manage Content List > reorder items and save.
    bool Items(Guid parent, List<ListSlotContentsInfo> list, string part);

    // UI test: Open Manage Content List; its header/metadata section loads this endpoint.
    List<ListSlotContentsInfo> ContentBlockHeader(Guid parent);
}
