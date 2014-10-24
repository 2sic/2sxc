using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav;

namespace ToSic.SexyContent
{
	public class SexyContentModuleUpgrade
	{
		public static string UpgradeModule(string version)
		{
			switch (version)
			{
				case "05.05.00":
					Version050500();
					break;
				case "06.03.07":
					Version060307();
					break;
			}

			// Increase ClientDependency version upon each upgrade (System and all Portals)
			// prevents browsers caching old JS and CSS files for editing, which could cause several errors
			ClientResourceManager.UpdateVersion();

			return version;
		}

		/// <summary>
		/// While upgrading to 05.04.02, make sure the template folders get renamed to "2sxc"
		/// </summary>
		private static void Version050500()
		{
			var portalController = new PortalController();
			var portals = portalController.GetPortals();
			var pathsToCopy = portals.Cast<PortalInfo>().Select(p => p.HomeDirectoryMapPath).ToList();
			pathsToCopy.Add(HttpContext.Current.Server.MapPath("~/Portals/_default/"));
			foreach (var path in pathsToCopy)
			{
				var portalFolder = new DirectoryInfo(path);
				if (portalFolder.Exists)
				{
					var oldSexyFolder = new DirectoryInfo(Path.Combine(path, "2sexy"));
					var newSexyFolder = new DirectoryInfo(Path.Combine(path, "2sxc"));
					var newSexyContentFolder = new DirectoryInfo(Path.Combine(newSexyFolder.FullName, "Content"));
					if (oldSexyFolder.Exists && !newSexyFolder.Exists)
					{
						// Move 2sexy directory to 2scx/Content
						DirectoryCopy(oldSexyFolder.FullName, newSexyContentFolder.FullName, true);

						// Leave info message in the content folder
						File.WriteAllText(Path.Combine(oldSexyFolder.FullName, "__WARNING - old copy of files - READ ME.txt"), "This is a short information\r\n\r\n2sxc renamed the main folder from \"[Portal]/2Sexy\" to \"[Portal]/2sxc\" in version 5.5.\r\n\r\nTo make sure that links to images/css/js still work, the old folder was copied and this was left. Please clean up and delete the entire \"[Portal]/2Sexy/\" folder once you're done. \r\n\r\nMany thanks!\r\n2sxc\r\n\r\nPS: Remember that you might have ClientDependency activated, so maybe you still have bundled & minified  JS/CSS-Files in your cache pointing to the old location. When done cleaning up, we recommend increasing the version just to be sure you're not seeing an old files that don't exist any more. ");

						// Move web.config (should be directly in 2sxc)
						if (File.Exists(Path.Combine(newSexyContentFolder.FullName, "web.config")))
							File.Move(Path.Combine(newSexyContentFolder.FullName, "web.config"), Path.Combine(newSexyFolder.FullName, "web.config"));

					}
				}
			}
		}

		/// <summary>
		/// Add new Content Types for Pipeline Designer
		/// </summary>
		private static void Version060307()
		{
			var eavVersionUpgrade = new VersionUpgrade(SexyContent.InternalUserName);
			eavVersionUpgrade.EnsurePipelineDesignerAttributeSets();
		}

		/// <summary>
		/// Copy a Directory recursive
		/// </summary>
		/// <remarks>Source: http://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx </remarks>
		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
			}

			// If the destination directory doesn't exist, create it. 
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location. 
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}
}