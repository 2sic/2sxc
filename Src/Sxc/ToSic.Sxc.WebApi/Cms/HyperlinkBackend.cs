using System;
using ToSic.Eav.Apps.Adam;
using ToSic.Eav.Run;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Cms
{
    internal class HyperlinkBackend: WebApiBackendBase<HyperlinkBackend>
    {
        public HyperlinkBackend() : base("Bck.HypLnk")
        {
        }

		public string ResolveHyperlink(IBlock block, string hyperlink, int appId, string contentType, Guid guid, string field)
		{
			try
			{
				// different security checks depending on the link-type
				var lookupPage = hyperlink.Trim().StartsWith("page", StringComparison.OrdinalIgnoreCase);

				// look it up first, because we need to know if the result is in ADAM or not (different security scenario)
				var conv = Eav.Factory.Resolve<IValueConverter>();
				var resolved = conv.ToValue(hyperlink, guid);

				if (lookupPage)
				{
					// page link - only resolve if the user has edit-permissions
					// only people who have some full edit permissions may actually look up pages
					var permCheckPage = new MultiPermissionsApp().Init(block.Context, GetApp(appId, block), Log);
					return permCheckPage.UserMayOnAll(GrantSets.WritePublished)
						? resolved
						: hyperlink;
				}

				// for file, we need guid & field - otherwise return the original unmodified
				if (guid == default || string.IsNullOrEmpty(field) || string.IsNullOrEmpty(contentType))
					return hyperlink;

				var isOutsideOfAdam = !(resolved.IndexOf($"/{AdamConstants.AdamRootFolder}/", StringComparison.Ordinal) > 0);

				// file-check, more abilities to allow
				// this will already do a ensure-or-throw inside it if outside of adam
				var adamCheck = new AdamState(block, appId, contentType, field, guid, isOutsideOfAdam, Log);
				if (!adamCheck.Security.SuperUserOrAccessingItemFolder(resolved, out var exp))
					throw exp;
				if (!adamCheck.Security.UserIsPermittedOnField(GrantSets.ReadSomething, out exp))
					throw exp;

				// if everything worked till now, it's ok to return the result
				return resolved;
			}
			catch
			{
				return hyperlink;
			}
		}
	}
}
