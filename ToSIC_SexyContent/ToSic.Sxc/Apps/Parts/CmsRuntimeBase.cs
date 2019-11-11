using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Apps
{
	public class CmsRuntimeBase: RuntimeBase
    {
        protected readonly CmsRuntime CmsRuntime;

        internal CmsRuntimeBase(CmsRuntime cmsRuntime, ILog parentLog) : base(cmsRuntime, parentLog)
        {
            CmsRuntime = cmsRuntime;
        }


    }

}