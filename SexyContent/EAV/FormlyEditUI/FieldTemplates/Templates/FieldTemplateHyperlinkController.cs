using System;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Security.Permissions;

namespace ToSic.SexyContent.EAV.FormlyEditUI
{
	[SupportedModules("2sxc,2sxc-app")]
	public class FieldTemplateHyperlinkController : SxcApiController
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
			return SexyContent.ResolveHyperlinkValues(hyperlink, Dnn.Portal);
		}

		//[HttpGet]
		//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
		//public FileInfo GetFileById(int fileId)
		//{
		//	var file = FileManager.Instance.GetFile(fileId);
		//	if (CanUserViewFile(file))
		//		return (FileInfo)file;

		//	return null;
		//}

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