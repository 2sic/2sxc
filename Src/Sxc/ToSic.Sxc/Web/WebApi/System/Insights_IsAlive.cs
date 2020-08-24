namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        public bool IsAlive()
        {
            ThrowIfNotSuperUser();
            return true;
        }
    }
}
