namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Raw properties of a UiPickerSourceEntity - also applies to UiPickerSourceQuery
/// Not complete, ATM just the props used in the code.
/// Later should probably split into IUiPickerSourceEntity and IUiPickerSourceQuery
/// </summary>
internal interface IUiPickerSourceEntity
{
    IEnumerable<IEntity> Query { get; }
    string CreateTypes { get; }
}