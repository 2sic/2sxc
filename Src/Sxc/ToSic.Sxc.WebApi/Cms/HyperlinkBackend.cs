﻿using System;
using ToSic.Eav.Apps.Adam;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.WebApi.Cms
{
    public class HyperlinkBackend<TFolderId, TFileId>: WebApiBackendBase<HyperlinkBackend<TFolderId, TFileId>>
    {
        private readonly Lazy<AdamContext<TFolderId, TFileId>> _adamState;
        private readonly IContextResolver _ctxResolver;
        private AdamContext<TFolderId, TFileId> AdamContext => _adamState.Value;

        public HyperlinkBackend(Lazy<AdamContext<TFolderId, TFileId>> adamState, IContextResolver ctxResolver, IServiceProvider serviceProvider) : base(serviceProvider, "Bck.HypLnk")
        {
            _adamState = adamState;
            _ctxResolver = ctxResolver.Init(Log);
        }

        // 2021-04-13 2dm should be unused now
        //public string ResolveHyperlink(int appId, string hyperlink, string contentType, Guid guid, string field)
        //{
        //	try
        //	{
        //		var context = _ctxResolver.BlockOrApp(appId);
        //		// different security checks depending on the link-type
        //		var lookupPage = hyperlink.Trim().StartsWith(ValueConverterBase.PrefixPage, StringComparison.OrdinalIgnoreCase);

        //		// look it up first, because we need to know if the result is in ADAM or not (different security scenario)
        //		var conv = ServiceProvider.Build<IValueConverter>();
        //		var resolved = conv.ToValue(hyperlink, guid);

        //		if (lookupPage)
        //		{
        //			// page link - only resolve if the user has edit-permissions
        //			// only people who have some full edit permissions may actually look up pages
        //			var permCheckPage = ServiceProvider.Build<MultiPermissionsApp>().Init(context, context.AppState, Log);
        //			return permCheckPage.UserMayOnAll(GrantSets.WritePublished)
        //				? resolved
        //				: hyperlink;
        //		}

        //		// for file, we need guid & field - otherwise return the original unmodified
        //		if (guid == default || string.IsNullOrEmpty(field) || string.IsNullOrEmpty(contentType))
        //			return hyperlink;

        //		var isOutsideOfAdam = !(resolved.IndexOf($"/{AdamConstants.AdamRootFolder}/", StringComparison.Ordinal) > 0);

        //		// file-check, more abilities to allow
        //		// this will already do a ensure-or-throw inside it if outside of adam
        //              var adamCheck = AdamContext; // new AdamState<int, int>();
        //              adamCheck.Init(context, contentType, field, guid, isOutsideOfAdam, Log);
        //		if (!adamCheck.Security.SuperUserOrAccessingItemFolder(resolved, out var exp))
        //			throw exp;
        //		if (!adamCheck.Security.UserIsPermittedOnField(GrantSets.ReadSomething, out exp))
        //			throw exp;

        //		// if everything worked till now, it's ok to return the result
        //		return resolved;
        //	}
        //	catch
        //	{
        //		return hyperlink;
        //	}
        //}

        public LinkInfoDto LookupHyperlink(int appId, string hyperlink, string contentType, Guid guid, string field)
        {
            try
            {
                // nothing to resolve
                if (string.IsNullOrEmpty(hyperlink))
                    return new LinkInfoDto { Value = hyperlink };

                var context = _ctxResolver.BlockOrApp(appId);
                // different security checks depending on the link-type
                var lookupPage = hyperlink.Trim().StartsWith(ValueConverterBase.PrefixPage, StringComparison.OrdinalIgnoreCase);

                // look it up first, because we need to know if the result is in ADAM or not (different security scenario)
                var conv = ServiceProvider.Build<IValueConverter>();
                var resolved = conv.ToValue(hyperlink, guid);

                if (lookupPage)
                {
                    // page link - only resolve if the user has edit-permissions
                    // only people who have some full edit permissions may actually look up pages
                    var permCheckPage = ServiceProvider.Build<MultiPermissionsApp>().Init(context, context.AppState, Log);
                    var userMay= permCheckPage.UserMayOnAll(GrantSets.WritePublished);
                    return new LinkInfoDto {Value = userMay ? resolved : hyperlink};
                }

                // for file, we need guid & field - otherwise return the original unmodified
                if (guid == default || string.IsNullOrEmpty(field) || string.IsNullOrEmpty(contentType))
                    return new LinkInfoDto { Value = hyperlink };

                var isOutsideOfAdam = !Sxc.Adam.Security.PathIsInItemAdam(guid, field, resolved);

                // file-check, more abilities to allow
                // this will already do a ensure-or-throw inside it if outside of adam
                var adamContext = AdamContext;
                adamContext.Init(context, contentType, field, guid, isOutsideOfAdam, Log);
                if (!adamContext.Security.SuperUserOrAccessingItemFolder(resolved, out var exp))
                    throw exp;
                if (!adamContext.Security.UserIsPermittedOnField(GrantSets.ReadSomething, out exp))
                    throw exp;
                
                // now try to find the item
                // we already know that the link was able to match, so we'll just use this to get the id
                var parts = new ValueConverterBase.LinkParts(hyperlink);
                // Note: kind of temporary solution, will fail if TFileId isn't int!
                var file = ((IAdamFileSystem<int, int>)adamContext.AdamManager.AdamFs).GetFile(parts.Id);
                var dtoMaker = AdamContext.ServiceProvider.Build<AdamItemDtoMaker<TFolderId, TFileId>>().Init(AdamContext);
                // if everything worked till now, it's ok to return the result
                var adam = dtoMaker.Create(file as File<TFolderId, TFileId>);
                return new LinkInfoDto {Adam = adam, Value = adam.Url};
            }
            catch
            {
                return new LinkInfoDto { Value = hyperlink };
            }
        }

	}
}
