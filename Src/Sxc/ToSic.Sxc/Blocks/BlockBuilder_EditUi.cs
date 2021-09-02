
namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        ///// <summary>
        ///// Activate the normal 2sxc read-js API - the $2sxc
        ///// </summary>
        //internal bool UiAddJsApi
        //{
        //    get => _uiAddJsApi ?? Block.Context.UserMayEdit;
        //    set
        //    {
        //        Log.Add($"UiAddJsApi:{value}");
        //        _uiAddJsApi = value;
        //    }
        //}

        //private bool? _uiAddJsApi;


        ///// <summary>
        ///// Activate the 2sxc commands API, the $2sxc(...).manage
        ///// </summary>
        //internal bool UiAddEditApi
        //{
        //    get => _uiAddEditApi ?? Block.Context.UserMayEdit;
        //    set
        //    {
        //        Log.Add($"UiAddEditApi:{value}");
        //        _uiAddEditApi = value;
        //    }
        //}

        //private bool? _uiAddEditApi;

        ///// <summary>
        ///// Activate the toolbar
        ///// </summary>
        //internal bool UiAddEditUi
        //{
        //    get => _uiAddEditUi ?? Block.Context.UserMayEdit;
        //    set
        //    {
        //        Log.Add($"UiAddEditApi:{value}");
        //        _uiAddEditUi = value;
        //    }
        //}

        //private bool? _uiAddEditUi;

        /// <summary>
        /// 
        /// </summary>
        internal bool UiAddEditContext
        {
            get => _uiAddEditContext ?? Block.Context.UserMayEdit;
            set
            {
                Log.Add($"UiAddEditContext:{value}");
                _uiAddEditContext = value;
            }
        }

        private bool? _uiAddEditContext;

        internal bool UiAutoToolbar
        {
            get => _uiAutoToolbar ?? Block.Context.UserMayEdit;
            set
            {
                Log.Add($"UiAutoToolbar:{value}");
                _uiAutoToolbar = value;
            }
        }

        private bool? _uiAutoToolbar;

    }
}
