using System.Collections.Generic;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private string PrepareUi(object ui, object uiMerge = default, string uiMergePrefix = default, IEnumerable<object> tweaks = default) 
            => Utils.PrepareUi(ui, uiMerge, uiMergePrefix, _configuration?.Group, tweaks: tweaks);
    }
}
