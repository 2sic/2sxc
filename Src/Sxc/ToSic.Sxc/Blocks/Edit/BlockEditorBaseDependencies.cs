using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Blocks.Edit
{
    public class BlockEditorBaseDependencies
    {
        public LazyInit<CmsRuntime> CmsRuntime { get; }
        public LazyInit<CmsManager> CmsManager { get; }
        public LazyInit<AppManager> AppManager { get; }

        public BlockEditorBaseDependencies(LazyInit<CmsRuntime> cmsRuntime, LazyInit<CmsManager> cmsManager, LazyInit<AppManager> appManager)
        {
            CmsRuntime = cmsRuntime;
            CmsManager = cmsManager;
            AppManager = appManager;
        }
    }
}
