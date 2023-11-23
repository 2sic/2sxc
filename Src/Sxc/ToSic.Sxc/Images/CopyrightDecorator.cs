using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Images
{
    // TODO: LOCATION / NAMESPACE not final
    [PrivateApi("WIP v16.08")]
    public class CopyrightDecorator: EntityBasedType
    {
        public static string TypeNameId = "077835ec-889e-433f-8acf-a4715acb3503";
        public static string NiceTypeName = "CopyrightDecorator";

        public CopyrightDecorator(IEntity entity) : base(entity)
        {
        }

        public string CopyrightInfoType => GetThis(null as string);

        public string CopyrightMessage => GetThis(null as string);

        // TODO: THIS will probably not work yet
        public IEnumerable<IEntity> Copyrights => GetThis(null as object) as IEnumerable<IEntity>;
    }
}
