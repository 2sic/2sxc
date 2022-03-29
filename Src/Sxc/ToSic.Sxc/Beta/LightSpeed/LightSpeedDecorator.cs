using ToSic.Eav.Data;

namespace ToSic.Sxc.Beta.LightSpeed
{
    public class LightSpeedDecorator: EntityBasedType
    {
        public static string TypeName = "be34f64b-7d1f-4ad0-b488-dabbbb01a186";
        public const string FieldIsEnabled = "IsEnabled";
        public const string FieldDuration = "Duration";

        public LightSpeedDecorator(IEntity entity) : base(entity)
        {
        }

        public bool IsEnabled => Get(FieldIsEnabled, false);

        public int Duration => Get(FieldDuration, 0);
    }
}
