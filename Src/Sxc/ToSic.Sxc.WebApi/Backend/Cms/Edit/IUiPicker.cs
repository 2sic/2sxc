namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Raw properties for a UI Picker like string-picker or entity-picker.
/// Not complete yet, just the fields currently used in the backend.
/// Later we probably need a IUiPickerString, IUiPickerEntity etc.
/// </summary>
internal interface IUiPicker
{
    string DataSources { get; }
}