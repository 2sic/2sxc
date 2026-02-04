using ToSic.Eav.Models;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context;

/// <summary>
/// Special Views for strongly typed code.
/// It supplies the Settings and Resources as strongly typed objects.
/// </summary>
/// <typeparam name="TSettings">Custom class for Settings</typeparam>
/// <typeparam name="TResources">Custom class for Resources</typeparam>
/// <remarks>
/// New v17.03
/// </remarks>
[PublicApi]
public interface ICmsView<out TSettings, out TResources> : ICmsView
    where TSettings : class, IModelOfData, new()
    where TResources : class, IModelOfData, new()
{
    /// <summary>
    /// All the app settings which are custom for each app.
    /// These are typed - typically to AppCode.Data.AppSettings
    /// </summary>
    new TSettings? Settings { get; }

    /// <summary>
    /// All the app resources (usually used for multi-language labels etc.).
    /// /// These are typed - typically to AppCode.Data.AppResources
    /// </summary>
    new TResources? Resources { get; }

}