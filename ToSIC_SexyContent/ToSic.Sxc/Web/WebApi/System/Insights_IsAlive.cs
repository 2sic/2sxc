namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Ins
    {
        public bool IsAlive()
        {
            ThrowIfNotSuperUser();
            return true;
        }
    }
}
