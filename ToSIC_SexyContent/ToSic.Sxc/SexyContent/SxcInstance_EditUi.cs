

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent
{
    public partial class CmsBlock
    {
        internal bool UiAddEditApi
        {
            get => _uiAddEditApi ?? UserMayEdit;
            set => _uiAddEditApi = value;
        }
        private bool? _uiAddEditApi;

        internal bool UiAddEditUi
        {
            get => _uiAddEditUi ?? UserMayEdit;
            set => _uiAddEditUi = value;
        }
        private bool? _uiAddEditUi;
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
