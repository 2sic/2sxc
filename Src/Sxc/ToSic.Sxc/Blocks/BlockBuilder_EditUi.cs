

using System.Collections.Generic;

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        /// <summary>
        /// Activate the normal 2sxc read-js API - the $2sxc
        /// </summary>
        internal bool UiAddJsApi
        {
            get => _uiAddJsApi ?? Block.Context.UserMayEdit;
            set
            {
                Log.Add($"{nameof(UiAddJsApi)}:{value}");
                _uiAddJsApi = value;
            }
        }

        private bool? _uiAddJsApi;


        /// <summary>
        /// Activate the 2sxc commands API, the $2sxc(...).manage
        /// </summary>
        internal bool UiAddEditApi
        {
            get => _uiAddEditApi ?? Block.Context.UserMayEdit;
            set
            {
                Log.Add($"{nameof(UiAddEditApi)}:{value}");
                _uiAddEditApi = value;
            }
        }

        private bool? _uiAddEditApi;

        /// <summary>
        /// Activate the toolbar
        /// </summary>
        internal bool UiAddEditUi
        {
            get => _uiAddEditUi ?? Block.Context.UserMayEdit;
            set
            {
                Log.Add($"{nameof(UiAddEditApi)}:{value}");
                _uiAddEditUi = value;
            }
        }

        private bool? _uiAddEditUi;

        /// <summary>
        /// 
        /// </summary>
        internal bool UiAddEditContext
        {
            get => _uiAddEditContext ?? Block.Context.UserMayEdit;
            set
            {
                Log.Add($"{nameof(UiAddEditContext)}:{value}");
                _uiAddEditContext = value;
            }
        }

        private bool? _uiAddEditContext;

        internal bool UiAutoToolbar
        {
            get => _uiAutoToolbar ?? Block.Context.UserMayEdit;
            set
            {
                Log.Add($"{nameof(UiAutoToolbar)}:{value}");
                _uiAutoToolbar = value;
            }
        }

        private bool? _uiAutoToolbar;


        internal List<string> NamedScriptsWIP = new List<string>();

        internal const string JsTurnOn = "turnOn";
    }
}
