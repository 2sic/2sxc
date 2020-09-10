using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.WebApi.Adam
{
    internal class AdamTransDelete : AdamTransactionBase<AdamTransDelete>
    {
        public AdamTransDelete() : base("Adm.TrnDel") { }

        internal bool Delete(string parentSubfolder, bool isFolder, int id)
        {
            Log.Add($"delete");
            if (!State.Security.UserIsPermittedOnField(GrantSets.DeleteSomething, out var exp))
                throw exp;

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!State.Security.UserIsNotRestrictedOrItemIsDraft(State.ItemGuid, out var permissionException))
                throw permissionException;

            // try to see if we can get into the subfolder - will throw error if missing
            var parent = State.ContainerContext.Folder(parentSubfolder, false);

            var fs = State.AdamAppContext.AdamFs;
            if (isFolder)
            {
                var target = fs.GetFolder(id);
                VerifySecurityAndStructure(State, parent, target, "can't delete folder");
                fs.Delete(target);
            }
            else
            {
                var target = fs.GetFile(id);
                VerifySecurityAndStructure(State, parent, target, "can't delete file");
                fs.Delete(target);
            }

            Log.Add("delete complete");
            return true;
        }

    }
}
