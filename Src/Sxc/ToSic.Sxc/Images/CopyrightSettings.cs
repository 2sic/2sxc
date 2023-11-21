using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Images
{
    // TODO: LOCATION / NAMESPACE not final
    [PrivateApi("WIP v16.08")]
    public class CopyrightSettings: EntityBasedType
    {
        public static string TypeNameId = "aed871cf-220b-4330-b368-f1259981c9c8";
        public static string NiceTypeName = "⚙️CopyrightSettings";

        public CopyrightSettings(IEntity entity) : base(entity)
        {
        }

        // todo: unclear if nullable bools work, probably never tested before
        public bool? ImagesInputEnabled => GetThis(null as bool?);

    }
}
