using ToSic.Eav.Data.Sys.Entities;

namespace ToSic.Sxc.Images.Sys;

// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
public record Copyright : RecordOfEntityBase
{
    public Copyright(IEntity entity) : base(entity) { }

    public static string TypeNameId = "ac3df5f0-c637-45e7-a52b-b323d50e52ac";
    public static string NiceTypeName = "🖺Copyright";

    public string? CopyrightType => GetThis<string>(null);

    public string? CopyrightMessage => GetThis<string>(null);

    public int CopyrightYear => GetThis(0);

    public string? CopyrightOwner => GetThis<string>(null);

    public string? CopyrightLink => GetThis<string>(null);

}