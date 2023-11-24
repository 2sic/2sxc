using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Images;

// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
public class Copyright: EntityBasedType
{
    public static string TypeNameId = "ac3df5f0-c637-45e7-a52b-b323d50e52ac";
    public static string NiceTypeName = "🖺Copyright";

    public Copyright(IEntity entity) : base(entity)
    {
    }

    public string CopyrightType => GetThis(null as string);

    public string CopyrightMessage => GetThis(null as string);

    public int CopyrightYear => GetThis(0);

    public string CopyrightOwner => GetThis(null as string);

    public string CopyrightLink => GetThis(null as string);

}