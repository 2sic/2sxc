using ToSic.Eav.Security.Internal;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Manager.Internal;
using ToSic.Sxc.Adam.Security.Internal;
using ToSic.Sxc.Adam.Work.Internal;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class HyperlinkBackend(
    LazySvc<AdamContext> adamCtxLazy,
    ISxcCurrentContextService ctxService,
    Generator<MultiPermissionsApp> appPermissions,
    Generator<IAdamItemDtoMaker, AdamItemDtoMakerOptions> adamDtoMaker,
    IValueConverter valueConverter)
    : ServiceBase("Bck.HypLnk", connect: [adamCtxLazy, appPermissions, ctxService, adamDtoMaker, valueConverter])
{
    public LinkInfoDto LookupHyperlink(int appId, string hyperlink, string contentType, Guid guid, string field)
    {
        try
        {
            // nothing to resolve
            if (string.IsNullOrEmpty(hyperlink))
                return new() { Value = hyperlink };

            var context = ctxService.GetExistingAppOrSet(appId);
            // different security checks depending on the link-type
            var lookupPage = hyperlink.Trim().StartsWith(ValueConverterBase.PrefixPage, StringComparison.OrdinalIgnoreCase);

            // look it up first, because we need to know if the result is in ADAM or not (different security scenario)
            var conv = valueConverter;
            var resolved = conv.ToValue(hyperlink, guid);

            if (lookupPage)
            {
                // page link - only resolve if the user has edit-permissions
                // only people who have some full edit permissions may actually look up pages
                var permCheckPage = appPermissions.New().Init(context, context.AppReader);
                var userMay= permCheckPage.UserMayOnAll(GrantSets.WritePublished);
                return new() {Value = userMay ? resolved : hyperlink};
            }

            // for file, we need guid & field - otherwise return the original unmodified
            if (guid == default || string.IsNullOrEmpty(field) || string.IsNullOrEmpty(contentType))
                return new() { Value = hyperlink };

            var isOutsideOfAdam = !AdamSecurity.PathIsInItemAdam(guid, field, resolved);

            // file-check, more abilities to allow
            // this will already do a ensure-or-throw inside it if outside of adam
            var adamCtx = adamCtxLazy.Value;
            adamCtx.Init(context, contentType, field, guid, isOutsideOfAdam);
            if (!adamCtx.Security.SuperUserOrAccessingItemFolder(resolved, out var exp))
                throw exp;
            if (!adamCtx.Security.UserIsPermittedOnField(GrantSets.ReadSomething, out exp))
                throw exp;
                
            // now try to find the item, use this to get the id
            var parts = new LinkParts(hyperlink);

            // link was not able to match,
            if (!parts.IsMatch || parts.Id == 0) 
                return new() { Value = hyperlink };

            // Note: kind of temporary solution, will fail if TFileId isn't int!
            var file = adamCtx.AdamManager.AdamFs.GetFile(AdamAssetIdentifier.Create(parts.Id));
            var dtoMaker = adamDtoMaker.New(new() { AdamContext = adamCtx });
            // if everything worked till now, it's ok to return the result
            var adam = dtoMaker.Create(file);
            return new() {Adam = adam, Value = adam.Url};
        }
        catch
        {
            return new() { Value = hyperlink };
        }
    }

}