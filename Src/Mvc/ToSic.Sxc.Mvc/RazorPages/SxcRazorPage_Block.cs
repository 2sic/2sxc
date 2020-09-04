using System;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Mvc.Code;

namespace ToSic.Sxc.Mvc.RazorPages
{
    public partial class SxcRazorPage<TModel>: IIsSxcRazorPage
    {
        #region DynCode 

        public DynamicCodeRoot DynCode
        {
            get => _dynCode ?? (_dynCode = new MvcDynamicCode().Init(Block, Log));
            set => _dynCode = value;
        }

        private DynamicCodeRoot _dynCode;
        #endregion

        public virtual IBlock Block 
        {
            get
            {
                if (_block != null) return _block;
                if(_dynCode == null) throw new Exception($"{nameof(BlockBuilder)} is empty, and DynCode isn't created - can't continue. Requires DynCode to be attached");
                return _block = _dynCode.Block;
            }
            protected set => _block = value;
        }
        protected IBlock _block;

    }
}
