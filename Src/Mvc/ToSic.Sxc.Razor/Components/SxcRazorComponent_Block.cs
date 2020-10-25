using System;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Razor.Code;

namespace ToSic.Sxc.Razor.Components
{
    public partial class SxcRazorComponent<TModel>: ISxcRazorComponent
    {
        #region DynCode 

        public DynamicCodeRoot DynCode
        {
            get => _dynCode ??= new Razor3DynamicCode().Init(Block, Log);
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
