﻿using ToSic.Eav.Data;

namespace ToSic.Sxc.Beta.LightSpeed
{
    public class LightSpeedDecorator: EntityBasedType
    {
        public static string TypeName = "be34f64b-7d1f-4ad0-b488-dabbbb01a186";
        public const string FieldIsEnabled = "IsEnabled";
        public const string FieldDuration = "Duration";
        public const string FieldDurationUser = "DurationUsers";
        public const string FieldDurationEditor = "DurationEditors";
        public const string FieldDurationSysAdmin = "DurationSystemAdmin";
        public const string FieldByUrlParameters = "ByUrlParameters";

        public LightSpeedDecorator(IEntity entity) : base(entity)
        {
        }

        public bool IsEnabled => Get(FieldIsEnabled, false);

        public int Duration => Get(FieldDuration, 0);

        public int DurationUser => Get(FieldDurationUser, 0);

        public int DurationEditor => Get(FieldDurationEditor, 0);

        public int DurationSystemAdmin => Get(FieldDurationSysAdmin, 0);

        public bool ByUrlParam => Get(FieldByUrlParameters, false);
    }
}