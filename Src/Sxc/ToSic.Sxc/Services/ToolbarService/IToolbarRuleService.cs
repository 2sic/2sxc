using ToSic.Eav.Documentation;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    [PrivateApi("WIP 13")]
    public interface IToolbarRuleService
    {
        /// <summary>
        /// WIP!
        /// This creates a MetadataRule for the Toolbar.
        /// It will take a metadata target, determine if the metadata already exists or not, and then create an edit-or-create rule for the JS inpage editing
        ///
        /// This corresponds to the `metadata` command in the InPage API
        /// 
        /// TODO: probably auto-detect which metadata is possible...?
        /// </summary>
        /// <param name="target"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        ToolbarRuleBase Metadata(
            object target,
            string contentType);
    }
}
