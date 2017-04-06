using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.SexyContent.Environment.Dnn7.EavImplementation;

namespace ToSic.SexyContent.WebApi.Dnn
{
	[SupportedModules("2sxc,2sxc-app")]
	public class HyperlinkController : SxcApiController
	{

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
		public object GetFileByPath(string relativePath)
		{
			relativePath = relativePath.Replace(Dnn.Portal.HomeDirectory, "");
			var file = FileManager.Instance.GetFile(Dnn.Portal.PortalId, relativePath);
			if (CanUserViewFile(file))
				return new
				{
					FileId= file.FileId
				};

			return null;
		}

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
		public string ResolveHyperlink(string hyperlink)
		{
            var conv = new DnnValueConverter();
		    return conv.Convert(ConversionScenario.GetFriendlyValue, "Hyperlink", hyperlink);
            // return SexyContent.ResolveHyperlinkValues(hyperlink, Dnn.Portal);
		}

		private bool CanUserViewFile(IFileInfo file)
		{
			if (file != null)
			{
				var folder = (FolderInfo)FolderManager.Instance.GetFolder(file.FolderId);
				if (FolderPermissionController.CanViewFolder(folder))
					return true;
			}

			return false;
		}

	}
}