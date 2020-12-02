using System;
using ToSic.Eav.Apps.Adam;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.WebApi.Cms
{
    public class HyperlinkBackend<TFolderId, TFileId>: WebApiBackendBase<HyperlinkBackend<TFolderId, TFileId>>
    {
        private readonly Lazy<AdamState<TFolderId, TFileId>> _adamState;
        private readonly IContextResolver _ctxResolver;
        private AdamState<TFolderId, TFileId> AdamState => _adamState.Value;

        public HyperlinkBackend(Lazy<AdamState<TFolderId, TFileId>> adamState, IContextResolver ctxResolver, IServiceProvider serviceProvider) : base(serviceProvider, "Bck.HypLnk")
        {
            _adamState = adamState;
            _ctxResolver = ctxResolver.Init(Log);
        }

		public string ResolveHyperlink(int appId, string hyperlink, string contentType, Guid guid, string field)
		{
			try
			{
				var context = _ctxResolver.App(appId);
				// different security checks depending on the link-type
				var lookupPage = hyperlink.Trim().StartsWith("page", StringComparison.OrdinalIgnoreCase);

				// look it up first, because we need to know if the result is in ADAM or not (different security scenario)
				var conv = ServiceProvider.Build<IValueConverter>();
				var resolved = conv.ToValue(hyperlink, guid);

				if (lookupPage)
				{
					// page link - only resolve if the user has edit-permissions
					// only people who have some full edit permissions may actually look up pages
					var permCheckPage = ServiceProvider.Build<MultiPermissionsApp>().Init(context, context.AppState, Log);
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
                var adamCheck = AdamState; // new AdamState<int, int>();
                adamCheck.Init(context, contentType, field, guid, isOutsideOfAdam, Log);
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
