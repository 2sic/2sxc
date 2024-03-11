using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context;

[PrivateApi("WIP v17")]
public interface ICmsView<out TSettings, out TResources> : ICmsView
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{
    /// <summary>
    /// All the app settings which are custom for each app.
    /// These are typed - typically to AppCode.Data.AppSettings
    /// </summary>
    new TSettings Settings { get; }

    /// <summary>
    /// All the app resources (usually used for multi-language labels etc.).
    /// /// These are typed - typically to AppCode.Data.AppResources
    /// </summary>
    new TResources Resources { get; }

}