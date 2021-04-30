using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared;

// TODO: #Oqtane - must provide additional sources like Context (http) etc.

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtGetLookupEngine : HasLog<ILookUpEngineResolver>, ILookUpEngineResolver
    {
        private readonly Lazy<QueryStringLookUp> _queryStringLookUp;
        private readonly Lazy<SiteLookUp> _siteLookUp;
        private readonly Lazy<PageLookUp> _pageLookUp;
        private readonly Lazy<ModuleLookUp> _moduleLookUp;
        private readonly Lazy<UserLookUp> _userLookUp;

        public OqtGetLookupEngine(
            Lazy<QueryStringLookUp> queryStringLookUp,
            Lazy<SiteLookUp> siteLookUp,
            Lazy<PageLookUp> pageLookUp,
            Lazy<ModuleLookUp> moduleLookUp,
            Lazy<UserLookUp> userLookUp) : base($"{OqtConstants.OqtLogPrefix}.LookUp")
        {
            _queryStringLookUp = queryStringLookUp;
            _siteLookUp = siteLookUp;
            _pageLookUp = pageLookUp;
            _moduleLookUp = moduleLookUp;
            _userLookUp = userLookUp;
        }

        public ILookUpEngine GetLookUpEngine(int instanceId/*, ILog parentLog*/)
        {
            var providers = new LookUpEngine(Log);

            providers.Add(_queryStringLookUp.Value);
            providers.Add(new DateTimeLookUp());
            providers.Add(new TicksLookUp());

            providers.Add(_siteLookUp.Value);
            providers.Add(_pageLookUp.Value);
            providers.Add(_moduleLookUp.Value);
            providers.Add(_userLookUp.Value);

            return providers;
        }
    }

    public class QueryStringLookUp : LookUpBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IQueryCollection _source;

        public QueryStringLookUp(IHttpContextAccessor httpContextAccessor)
        {
            Name = "Query";
            _httpContextAccessor = httpContextAccessor;
        }

        public override string Get(string key, string format)
        {
            _source ??= _httpContextAccessor?.HttpContext?.Request.Query;
            if (_source == null) return string.Empty;
            return _source.TryGetValue(key, out var result) ? result.ToString() : string.Empty;
        }
    }

    public class DateTimeLookUp : LookUpBase
    {
        public DateTimeLookUp()
        {
            Name = "DateTime";
        }

        public override string Get(string key, string format)
        {
            return key.ToLowerInvariant() switch
            {
                "now" => DateTime.Now.ToString(format),
                _ => string.Empty
            };
        }
    }

    public class TicksLookUp : LookUpBase
    {
        public TicksLookUp()
        {
            Name = "Ticks";
        }

        public override string Get(string key, string format)
        {
            return key.ToLowerInvariant() switch
            {
                "now" => DateTime.Now.Ticks.ToString(format),
                "today" => DateTime.Today.Ticks.ToString(format),
                "ticksperday" => TimeSpan.TicksPerDay.ToString(format),
                _ => string.Empty
            };
        }
    }

    public class UserLookUp : LookUpBase
    {
        private readonly Lazy<IUserResolver> _userResolver;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Oqtane.Models.User User { get; set; }
        private Oqtane.Models.User UserDB { get; set; }

        public UserLookUp(Lazy<IUserResolver> userResolver, Lazy<IUserRepository> userRepository, IHttpContextAccessor httpContextAccessor)
        {
            Name = "User";
            _userResolver = userResolver;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override string Get(string key, string format)
        {
            try
            {
                User ??= _userResolver.Value.GetUser();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{User.UserId}",
                    "username" => $"{User.Username}",
                    "displayname" => $"{(UserDB ??=_userRepository.Value.GetUser(User.UserId))?.DisplayName}",
                    "email" => $"{(UserDB ??= _userRepository.Value.GetUser(User.UserId))?.Email}",
                    "guid" => $"{_httpContextAccessor?.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value}",
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public class ModuleLookUp : LookUpBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IServiceProvider _serviceProvider;
        //private HttpRequest _GetRequest() => _httpContextAccessor.HttpContext.Request;
        private IDictionary<object, object?> _items;
        private Oqtane.Models.Module Module { get; set; }

        public ModuleLookUp(IHttpContextAccessor httpContextAccessor/*, IServiceProvider serviceProvider*/)
        {
            Name = "Module";

            _httpContextAccessor = httpContextAccessor;
            //_serviceProvider = serviceProvider;
        }

        public Oqtane.Models.Module GetSource()
        {
            //var oqtState = new OqtState(_GetRequest, _serviceProvider, Log);
            //var ctx = oqtState.GetContext();
            //var module = (OqtModule)ctx.Module;
            //return module;

            // HACK: WIP
            _items ??= _httpContextAccessor?.HttpContext.Items;
            if (_items == null) return null;

            return !_items.TryGetValue(Name + "ForLookUp", out var module) ? null : module as Oqtane.Models.Module;
        }

        public override string Get(string key, string format)
        {
            try
            {
                Module ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Module.ModuleId}",
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public class PageLookUp : LookUpBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<SiteState> _siteState;


        //private HttpRequest _GetRequest() => _httpContextAccessor.HttpContext.Request;
        private IDictionary<object, object?> _items;
        protected Oqtane.Models.Page Page { get; set; }

        public PageLookUp(IHttpContextAccessor httpContextAccessor/*, IServiceProvider serviceProvider*/, Lazy<SiteState> siteState)
        {
            Name = "Page";

            _httpContextAccessor = httpContextAccessor;
            _siteState = siteState;
            //_serviceProvider = serviceProvider;
        }

        public Oqtane.Models.Page GetSource()
        {
            //var oqtState = new OqtState(_GetRequest, _serviceProvider, Log);
            //var ctx = oqtState.GetContext();
            //return ctx.Page;

            // HACK: WIP
            _items ??= _httpContextAccessor?.HttpContext.Items;
            if (_items == null) return null;

            return !_items.TryGetValue(Name + "ForLookUp", out var page) ? null : page as Oqtane.Models.Page;
        }

        public override string Get(string key, string format)
        {
            try
            {
                Page ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Page.PageId}",
                    "url" => (_siteState?.Value?.Alias != null) ? $"//{_siteState?.Value?.Alias?.Path}/{Page.Path}" : string.Empty,
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public class SiteLookUp : LookUpBase
    {
        public SiteState SiteState { get; }
        protected Oqtane.Models.Site Site { get; set; }
        private readonly Lazy<SiteStateInitializer> _siteStateInitializer;
        private readonly Lazy<SiteRepository> _siteRepository;

        public SiteLookUp(Lazy<SiteStateInitializer> siteStateInitializer, SiteState siteState, Lazy<SiteRepository> siteRepository)
        {
            Name = "Site";
            SiteState = siteState;
            _siteStateInitializer = siteStateInitializer;
            _siteRepository = siteRepository;
        }

        public Oqtane.Models.Site GetSource()
        {
            if (!_siteStateInitializer.Value.InitIfEmpty()) return null;
            var site = _siteRepository.Value.GetSite(SiteState.Alias.SiteId);
            //var oqtSite = _serviceProvider.Build<OqtSite>().Init(site);

            return site;
        }

        public override string Get(string key, string format)
        {
            try
            {
                Site ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Site.SiteId}",
                    "guid" => $"{Site.SiteGuid}",
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }

}
