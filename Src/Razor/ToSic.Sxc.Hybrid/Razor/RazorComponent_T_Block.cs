using ToSic.Sxc.Code;
using ToSic.Sxc.Hybrid;

namespace ToSic.Custom
{
    public partial class RazorComponent<TModel>: IHybridRazorComponent
    {
        #region DynCode

        public IDynamicCodeRoot _DynCodeRoot
        {
            get => _dynCode;// 2dm I don't think this inner call would ever work, as it needs DynCode to work ??= GetService<DynamicCodeRoot>().Init(Block, Log);
            set => _dynCode = value;
        }

        private IDynamicCodeRoot _dynCode;
        #endregion

        //public virtual IBlock Block 
        //{
        //    get
        //    {
        //        if (_block != null) return _block;
        //        if(_dynCode == null) throw new Exception($"{nameof(BlockBuilder)} is empty, and DynCode isn't created - can't continue. Requires DynCode to be attached");
        //        return _block = _dynCode._Block;
        //    }
        //    protected set => _block = value;
        //}
        //protected IBlock _block;

    }
}
