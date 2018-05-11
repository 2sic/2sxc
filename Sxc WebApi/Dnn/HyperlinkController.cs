using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7.EavImplementation;
using ToSic.SexyContent.WebApi.Adam;
using ToSic.SexyContent.WebApi.Permissions;
using Type = ToSic.Eav.WebApi.Formats.Type;

namespace ToSic.SexyContent.WebApi.Dnn
{
	[SupportedModules("2sxc,2sxc-app")]
	public class HyperlinkController : SxcApiControllerBase
	{

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
		public object GetFileByPath(string relativePath)
		{
		    var context = GetContext(SxcInstance, Log);
            relativePath = relativePath.Replace(context.Dnn.Portal.HomeDirectory, "");
			var file = FileManager.Instance.GetFile(context.Dnn.Portal.PortalId, relativePath);
			if (CanUserViewFile(file))
				return new
				{
				    file.FileId
				};

			return null;
		}

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
		public string ResolveHyperlink(string hyperlink, int appId, string contentType, Guid guid, string field)
		{
		    try
		    {
		        // different security checks depending on the link-type
		        var lookupPage = hyperlink.Trim().StartsWith("page", StringComparison.OrdinalIgnoreCase);

		        // look it up first, because we need to know if the result is in ADAM or not (different security scenario)
		        var conv = new DnnValueConverter();
		        var resolved = conv.Convert(ConversionScenario.GetFriendlyValue, "Hyperlink", hyperlink);

		        if (lookupPage)
		        {
                    // page link - only resolve if the user has edit-permissions
		            // only people who have some full edit permissions may actually look up pages
		            var permCheckPage = new AppAndPermissions(SxcInstance, appId, Log);
		            if (!permCheckPage.Ensure(GrantSets.WritePublished, null, out var exp))
		                throw exp;
		            return resolved;
		        }

		        var isOutsideOfAdam = !(resolved.IndexOf("/adam/", StringComparison.Ordinal) > 0);

		        // file-check, more abilities to allow
		        // this will already do a ensure-or-throw inside it if outside of adam
		        var adamCheck = new AdamSecureState(SxcInstance, appId, contentType, field, guid, isOutsideOfAdam, Log);
                adamCheck.ThrowIfRestrictedUserIsOutsidePermittedFolders(resolved);
                adamCheck.ThrowIfRestrictedUserIsntPermittedOnField(GrantSets.ReadSomething);

		        // if everythig worked till now, it's ok to return the result
		        return resolved;
		    }
		    catch
		    {
		        return hyperlink;
		    }
		}

		private static bool CanUserViewFile(IFileInfo file)
		{
		    if (file == null) return false;
		    var folder = (FolderInfo)FolderManager.Instance.GetFolder(file.FolderId);
		    return FolderPermissionController.CanViewFolder(folder);
		}

	}
}