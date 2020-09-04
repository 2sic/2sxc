using System;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Mvc.Dev;

namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public abstract class SxcTestPageViewParams<TModel>: SxcRazorPage<TModel>
    {
        protected SxcTestPageViewParams()
        {
            Log.Rename("Mvc.SxcRzr");
        }

        #region Properties to identify the block

        public int Id => _id ?? (_id = GetNumberFromViewData("Id")).Value;
        private int? _id;

        public int PageId => _pageId ?? (_pageId = GetNumberFromViewData("PageId")).Value;
        private int? _pageId;

        public int AppId => _appId ?? (_appId = GetNumberFromViewData("AppId")).Value;
        private int? _appId;

        public Guid BlockGuid => _blockGuid ?? (_blockGuid = GetGuidFromViewData("Block")).Value;
        private Guid? _blockGuid;

        private int GetNumberFromViewData(string name)
        {
            object idObj = null;
            ViewData?.TryGetValue(name, out idObj);
            int.TryParse(idObj?.ToString(), out var tempId);
            return tempId;
        }
        private Guid GetGuidFromViewData(string name)
        {
            object idObj = null;
            ViewData?.TryGetValue(name, out idObj);
            Guid.TryParse(idObj?.ToString(), out var tempId);
            return tempId;
        }

        #endregion


        public override IBlock Block 
        {
            get
            {
                if (_block != null) return _block;
                _block = SxcMvc.CreateBuilder(TestIds.PrimaryZone, PageId, Id, AppId, BlockGuid, Log);
                return _block;
            }
        }

    }
}
