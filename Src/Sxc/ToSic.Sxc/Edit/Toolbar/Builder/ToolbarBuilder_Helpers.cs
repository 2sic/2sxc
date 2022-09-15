namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private string PrepareUi(object ui, object uiMerge = null, string uiMergePrefix = null) 
            => Utils.PrepareUi(ui, uiMerge, uiMergePrefix, _configuration?.Group);
    }
}
