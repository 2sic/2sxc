using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.Parameters;


namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtGetLookupEngine : HasLog<ILookUpEngineResolver>, ILookUpEngineResolver
    {
        #region Constructor and DI

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
        private readonly Lazy<QueryStringLookUp> _queryStringLookUp;
        private readonly Lazy<SiteLookUp> _siteLookUp;
        private readonly Lazy<PageLookUp> _pageLookUp;
        private readonly Lazy<ModuleLookUp> _moduleLookUp;
        private readonly Lazy<UserLookUp> _userLookUp;

        #endregion

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
            Name = "QueryString";
            _httpContextAccessor = httpContextAccessor;
        }

        public override string Get(string key, string format)
        {
            _source ??= _httpContextAccessor?.HttpContext?.Request.Query;
            if (_source == null) return string.Empty;

            // Special handling when having original parameters in query string.
            var overrideParam = GetOverrideParam(key);
            if (!string.IsNullOrEmpty(overrideParam)) return overrideParam;

            return _source.TryGetValue(key, out var result) ? result.ToString() : string.Empty;
        }

        private string GetOverrideParam(string key)
        {
            if (!_source.TryGetValue(OriginalParameters.NameInUrlForOriginalParameters, out var queryStringValue))
                return string.Empty;

            var originalParams = new List<KeyValuePair<string, string>> 
                { new(OriginalParameters.NameInUrlForOriginalParameters, queryStringValue.ToString()) };

            var overrideParams = OriginalParameters.GetOverrideParams(originalParams)
                .Where(l => l.Key == key.ToLowerInvariant())
                .ToList();

            return overrideParams.Any() ? overrideParams.First().Value : string.Empty;
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
        private readonly OqtUser _oqtUser;

        public UserLookUp(IUser oqtUser)
        {
            Name = "User";

            _oqtUser = oqtUser as OqtUser;
        }

        public override string Get(string key, string format)
        {
            try
            {
                return key.ToLowerInvariant() switch
                {
                    "id" => $"{_oqtUser.Id}",
                    "username" => $"{_oqtUser.UnwrappedContents.Username}",
                    "displayname" => $"{_oqtUser.UnwrappedContents.DisplayName}",
                    "email" => $"{_oqtUser.UnwrappedContents.Email}",
                    "guid" => $"{_oqtUser.Guid}",

                    //"issuperuser" => $"{_oqtUser.IsSuperUser}",
                    //"isadmin" => $"{_oqtUser.IsAdmin}",
                    //"isanonymous" => $"{_oqtUser.IsAnonymous}",

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
        private readonly OqtState _oqtState;
        private Oqtane.Models.Module Module { get; set; }

        public ModuleLookUp(OqtState oqtState)
        {
            Name = "Module";

            _oqtState = oqtState;
        }

        public Module GetSource()
        {
            var ctx = _oqtState.GetContext();
            var module = (OqtModule)ctx.Module;
            return module.UnwrappedContents;
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
        private readonly Lazy<OqtState> _oqtState;
        protected Oqtane.Models.Page Page { get; set; }

        public PageLookUp(Lazy<OqtState> oqtState)
        {
            Name = "Page";

            _oqtState = oqtState;
        }

        public Oqtane.Models.Page GetSource()
        {
            var ctx = _oqtState.Value.GetContext();
            return ((OqtPage) ctx.Page).UnwrappedContents;
        }

        public override string Get(string key, string format)
        {
            try
            {
                Page ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Page.PageId}",
                    "url" => $"{Page.Url}",
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
