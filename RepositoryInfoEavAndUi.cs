using System.Collections.Generic;
using System.Web;
using ToSic.Eav.Repositories;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Provides global information about where the folders are which should be loaded in this environment
    /// - the .data/
    /// - the dist/edit/.data/
    /// - the dist/sxc-edit/.data/
    /// - the .databeta (this is for testing only, will never be in the distribution)
    /// </summary>
    /// <remarks>
    /// Is used by reflection, so you won't see any direct references to this anywhere
    /// </remarks>
    public class RepositoryInfoEavAndUi: RepositoryInfoOfFolder
    {
        //public RepositoryInfoEavAndUi() : base(true, true, null) { }

        public override List<string> RootPaths => new List<string>
        {
            BuildPath(".data"),
            BuildPath("dist/edit/.data"),
            BuildPath("dist/sxc-edit/.data"),
            BuildPath(".databeta")
        };

        private string BuildPath(string pathEnd) =>
            HttpContext.Current.Server.MapPath(System.IO.Path.Combine(Eav.ImportExport.Settings.ModuleDirectory,
                pathEnd));
    }
}