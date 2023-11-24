using System;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Dnn.Features
{
    public class SysFeatureDnn: SysFeatureDetector
    {

        private static readonly SysFeature DefStatic = new(
                "Dnn",
                new Guid("00cb8d98-bfac-4a18-b7de-b1237498f183"),
                "Dnn"
            );

        public SysFeatureDnn() : base(DefStatic, true) { }

    }
}
