

namespace ToSic.Sxc.Blocks
{
    public partial class CmsBlock
    {
        /// <summary>
        /// Activate the normal 2sxc read-js API - the $2sxc
        /// </summary>
        internal bool UiAddJsApi
        {
            get => _uiAddJsApi ?? UserMayEdit;
            set => _uiAddJsApi = value;
        }
        private bool? _uiAddJsApi;


        /// <summary>
        /// Activate the 2sxc commands API, the $2sxc(...).manage
        /// </summary>
        internal bool UiAddEditApi
        {
            get => _uiAddEditApi ?? UserMayEdit;
            set => _uiAddEditApi = value;
        }
        private bool? _uiAddEditApi;

        /// <summary>
        /// Activate the toolbars
        /// </summary>
        internal bool UiAddEditUi
        {
            get => _uiAddEditUi ?? UserMayEdit;
            set => _uiAddEditUi = value;
        }
        private bool? _uiAddEditUi;

        /// <summary>
        /// 
        /// </summary>
        internal bool UiAddEditContext
        {
            get => _uiAddEditContext ?? UserMayEdit;
            set => _uiAddEditContext = value;
        }
        private bool? _uiAddEditContext;

        internal bool UiAutoToolbar
        {
            get => _uiAutoToolbar ?? UserMayEdit;
            set => _uiAutoToolbar = value;
        }
        private bool? _uiAutoToolbar;
    }
}
