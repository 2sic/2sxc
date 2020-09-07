using System.Linq;
using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.WebApi.Adam
{
    internal class AdamTransRename : AdamTransactionBase<AdamTransRename>
    {
        public AdamTransRename() : base("Adm.TrnDel") { }

        internal bool Rename(string parentSubfolder, bool isFolder, int id, string newName)
        {
            Log.Add($"");

            if (!State.Security.UserIsPermittedOnField(GrantSets.WriteSomething, out var exp))
                throw exp;

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!State.Security.UserIsNotRestrictedOrItemIsDraft(State.ItemGuid, out var permissionException))
                throw permissionException;

            // try to see if we can get into the subfolder - will throw error if missing
            var parent = State.ContainerContext.Folder(parentSubfolder, false);

            var fs = State.AdamAppContext.EnvironmentFs;
            if (isFolder)
            {
                var target = fs.GetFolder(id);
                VerifySecurityAndStructure(State, parent, target, target.Id, "Can't rename folder");
                fs.Rename(target, newName);
            }
            else
            {
                var target = fs.GetFile(id);
                VerifySecurityAndStructure(State, parent, target, target.FolderId, "Can't rename file");

                // never allow to change the extension
                if (target.Extension != newName.Split('.').Last())
                    newName += "." + target.Extension;
                fs.Rename(target, newName);
            }

            Log.Add("rename complete");
            return true;
        }
    }
}
