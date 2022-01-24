namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        public string Details(string view)
        {
            ThrowIfNotSuperUser();

            switch (view.ToLowerInvariant())
            {
                case "help": return Help();
                case "licenses": return Licenses();
                default: return $"Error: View name {view} unknown";
            }
        }
    }
}
