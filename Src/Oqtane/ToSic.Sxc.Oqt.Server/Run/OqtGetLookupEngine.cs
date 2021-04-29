using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Reflection;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared;
using Log = ToSic.Eav.Logging.Simple.Log;

// TODO: #Oqtane - must provide additional sources like Context (http) etc.

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtGetLookupEngine: HasLog<ILookUpEngineResolver>, ILookUpEngineResolver
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

    public abstract class ReflectionLookUpBase : LookUpBase, IHasLog
    {
        protected object Source { get; set; }
        private readonly BindingFlags _bindingFlags;

        protected ReflectionLookUpBase(BindingFlags bindingFlags = BindingFlags.Instance)
        {
            Log ??= new Log($"{OqtConstants.OqtLogPrefix}.{Name}LookUp");
            _bindingFlags = bindingFlags;
        }

        public ReflectionLookUpBase Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        public ILog Log { get; }

        [CanBeNull]
        public abstract object GetSource();

        public override string Get(string key, string format)
        {
            Source ??= GetSource();
            return Source == null ? string.Empty : GetValue(key, format);
        }

        private string GetValue(string key, string format)
        {
            try
            {
                var type = Source.GetType();
                var propertyInfo = type.GetProperty(key, BindingFlags.Public | BindingFlags.IgnoreCase | _bindingFlags);
                return propertyInfo != null ? string.Format($"{propertyInfo.GetValue(Source)}", format) : string.Empty;
            }
            catch (Exception e)
            {
                // TODO: WIP - make better exception handling
                return $"Error getting value for key:{key}. {e.Message}";
            }
        }
    }

    public class UserLookUp : ReflectionLookUpBase
    {
        private readonly Lazy<IUserResolver> _userResolver;

        public UserLookUp(Lazy<IUserResolver> userResolver) : base(BindingFlags.Instance)
        {
            Name = "User";
            _userResolver = userResolver;
        }

        public override object GetSource() => _userResolver.Value.GetUser();
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
            Module ??= GetSource();

            return key.ToLowerInvariant() switch
            {
                "id" => $"{Module.ModuleId}",
                _ => string.Empty
            };
        }
    }

    public class PageLookUp : LookUpBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IServiceProvider _serviceProvider;
        //private HttpRequest _GetRequest() => _httpContextAccessor.HttpContext.Request;
        private IDictionary<object, object?> _items;
        protected Oqtane.Models.Page page { get; set; }

        public PageLookUp(IHttpContextAccessor httpContextAccessor/*, IServiceProvider serviceProvider*/)
        {
            Name = "Page";

            _httpContextAccessor = httpContextAccessor;
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
            page ??= GetSource();

            return key.ToLowerInvariant() switch
            {
                "id" => $"{page.PageId}",
                _ => string.Empty
            };
        }
    }

    public class SiteLookUp : LookUpBase
    {
        public SiteState SiteState { get; }
        protected Oqtane.Models.Site site { get; set; }
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
            site ??= GetSource();

            return key.ToLowerInvariant() switch
            {
                "id" => $"{site.SiteId}",
                "guid" => $"{site.SiteGuid}",
                _ => string.Empty
            };
        }
    }

}
