using System;
using ToSic.Eav.Data;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HyperlinkBackend<TFolderId, TFileId>: ServiceBase
{
    private readonly Generator<AdamItemDtoMaker<TFolderId, TFileId>> _adamDtoMaker;
    private readonly IValueConverter _valueConverter;
    private readonly Generator<MultiPermissionsApp> _appPermissions;
    private readonly LazySvc<AdamContext<TFolderId, TFileId>> _adamState;
    private readonly ISxcContextResolver _ctxResolver;
    private AdamContext<TFolderId, TFileId> AdamContext => _adamState.Value;

    public HyperlinkBackend(
        LazySvc<AdamContext<TFolderId, TFileId>> adamState,
        ISxcContextResolver ctxResolver,
        Generator<MultiPermissionsApp> appPermissions,
        Generator<AdamItemDtoMaker<TFolderId, TFileId>> adamDtoMaker,
        IValueConverter valueConverter) : base("Bck.HypLnk")
    {
        _adamDtoMaker = adamDtoMaker;
        ConnectServices(
            _adamState = adamState,
            _appPermissions = appPermissions,
            _ctxResolver = ctxResolver,
            _valueConverter = valueConverter
        );
    }
        

    public LinkInfoDto LookupHyperlink(int appId, string hyperlink, string contentType, Guid guid, string field)
    {
        try
        {
            // nothing to resolve
            if (string.IsNullOrEmpty(hyperlink))
                return new LinkInfoDto { Value = hyperlink };

            var context = _ctxResolver.GetBlockOrSetApp(appId);
            // different security checks depending on the link-type
            var lookupPage = hyperlink.Trim().StartsWith(ValueConverterBase.PrefixPage, StringComparison.OrdinalIgnoreCase);

            // look it up first, because we need to know if the result is in ADAM or not (different security scenario)
            var conv = _valueConverter;
            var resolved = conv.ToValue(hyperlink, guid);

            if (lookupPage)
            {
                // page link - only resolve if the user has edit-permissions
                // only people who have some full edit permissions may actually look up pages
                var permCheckPage = _appPermissions.New().Init(context, context.AppState);
                var userMay= permCheckPage.UserMayOnAll(GrantSets.WritePublished);
                return new LinkInfoDto {Value = userMay ? resolved : hyperlink};
            }

            // for file, we need guid & field - otherwise return the original unmodified
            if (guid == default || string.IsNullOrEmpty(field) || string.IsNullOrEmpty(contentType))
                return new LinkInfoDto { Value = hyperlink };

            var isOutsideOfAdam = !Security.PathIsInItemAdam(guid, field, resolved);

            // file-check, more abilities to allow
            // this will already do a ensure-or-throw inside it if outside of adam
            var adamContext = _adamState.Value;
            adamContext.Init(context, contentType, field, guid, isOutsideOfAdam, cdf: null);
            if (!adamContext.Security.SuperUserOrAccessingItemFolder(resolved, out var exp))
                throw exp;
            if (!adamContext.Security.UserIsPermittedOnField(GrantSets.ReadSomething, out exp))
                throw exp;
                
            // now try to find the item, use this to get the id
            var parts = new LinkParts(hyperlink);

            // link was not able to match,
            if (!parts.IsMatch || parts.Id == 0) 
                return new LinkInfoDto { Value = hyperlink };

            // Note: kind of temporary solution, will fail if TFileId isn't int!
            var file = ((IAdamFileSystem<int, int>)adamContext.AdamManager.AdamFs).GetFile(parts.Id);
            var dtoMaker = _adamDtoMaker.New().Init(AdamContext);
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