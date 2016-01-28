using ToSic.SexyContent.Environment.Interfaces;

namespace ToSic.SexyContent.Environment.None
{
    public class Permissions: IPermissions
    {
        public bool UserMayEditContent { get { return false; } }
    }
}