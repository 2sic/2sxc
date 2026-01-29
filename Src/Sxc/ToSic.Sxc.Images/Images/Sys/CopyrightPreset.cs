using ToSic.Eav.Models;

namespace ToSic.Sxc.Images.Sys;

/// <summary>
/// This is a copyright information type.
/// Each entity contains one copyright, such as "MIT" or "CC BY-NC-SA".
/// These can be defined in each system and then selected when adding copyright info to an image.
/// </summary>
// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
[ModelSpecs(ContentType = ContentTypeNameId)]
public record CopyrightPreset : ModelOfEntityCore
{
    public const string ContentTypeNameId = "ac3df5f0-c637-45e7-a52b-b323d50e52ac";
    public const string ContentTypeName = "🖺Copyright";


    /// <summary>
    /// The title.
    /// </summary>
    public string? CopyrightMessage => GetThis<string>(null);

    #region Not used fields - these fields never show in the UI, so we can probably remove them some day 2026-01

    /// <summary>
    /// The primary type, such as "MIT" or "CC BY-NC-SA". Can also be "none" or "copyright" without further details.
    /// </summary>
    public string? CopyrightType => GetThis<string>(null);

    public int CopyrightYear => GetThis(0);

    public string? CopyrightOwner => GetThis<string>(null);

    public string? CopyrightLink => GetThis<string>(null);

    #endregion
}