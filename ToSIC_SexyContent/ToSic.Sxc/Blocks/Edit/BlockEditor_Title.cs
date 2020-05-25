using System.Linq;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Blocks
{
    public partial class BlockEditorBase
    {
        internal void UpdateTitle()
        {
            Log.Add("update title");
            // check the blockConfiguration as to what should be the module title, then try to set it
            // technically it could have multiple different groups to save in, 
            // ...but for now we'll just update the current modules title
            // note: it also correctly handles published/unpublished, but I'm not sure why :)

            var cms = GetCmsRuntime();
            var contentGroup = cms.Blocks.GetBlockConfig(BlockConfiguration.Guid);

            var titleItem = contentGroup.Header.FirstOrDefault() ?? contentGroup.Content.FirstOrDefault();

            UpdateTitle(titleItem);
        }

        private CmsRuntime GetCmsRuntime()
            // todo: this must be changed, set showDrafts to true for now, as it's probably only used in the view-picker, but it shoudln't just be here
            => BlockBuilder.App == null ? null : new CmsRuntime(BlockBuilder.App, Log, true, false);

    }
}
