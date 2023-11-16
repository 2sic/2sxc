using System;
using ToSic.Eav.Run.Capabilities;

namespace ToSic.Sxc.Dnn.Run.Capabilities
{
    public class DnnCapability: SysFeatureDetector
    {

        private static readonly SystemCapabilityDefinition DefStatic = new SystemCapabilityDefinition(
                "Dnn",
                new Guid("00cb8d98-bfac-4a18-b7de-b1237498f183"),
                "Dnn"
            );

        public DnnCapability() : base(DefStatic, true) { }

    }
}
