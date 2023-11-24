using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Apps;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IAppConfiguration: IEntityBasedType
{
    /// <summary>
    /// Version of the App.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// The visible name of the App.
    /// Note that it can be localized.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description of the App.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// App Folder.
    /// </summary>
    string Folder { get; }

    /// <summary>
    /// Does the App support Razor templates?
    /// This is more informative and doesn't have an effect.
    /// </summary>
    [PrivateApi("Hide as it doesn't have any real effect.")]
    bool EnableRazor { get; }
    [PrivateApi("Hide as it doesn't have any real effect.")]
    bool EnableToken { get; }

    /// <summary>
    /// If the App is hidden, it won't be visible in the add-another-app dialog.
    /// </summary>
    bool IsHidden { get; }

    /// <summary>
    /// If the App supports AJAX reload on edit.
    /// Most Apps do; some with complex JavaScript don't.
    /// </summary>
    bool EnableAjax { get; }

    [PrivateApi("Hide as it's not really important.")]
    Guid OriginalId { get; }

    /// <summary>
    /// 
    /// </summary>
    Version RequiredSxc { get; }
    Version RequiredDnn { get; }
    Version RequiredOqtane { get; }
}