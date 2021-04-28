using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Shared;

// TODO: #Oqtane - must provide additional sources like Context (http) etc.

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtGetLookupEngine: HasLog<ILookUpEngineResolver>, ILookUpEngineResolver
    {
        private readonly Lazy<LookUpInQueryString> _lookUpInQueryString;

        public OqtGetLookupEngine(Lazy<LookUpInQueryString> lookUpInQueryString) : base($"{OqtConstants.OqtLogPrefix}.LookUp")
        {
            _lookUpInQueryString = lookUpInQueryString;
        }

        public ILookUpEngine GetLookUpEngine(int instanceId/*, ILog parentLog*/)
        {
            var providers = new LookUpEngine(Log);

            var dummy = new Dictionary<string, string>();
            dummy.Add("Ivo", "Ivić");
            dummy.Add("Pero","Perić");

            providers.Add(new LookUpInDictionary("dummy", dummy));

            providers.Add(_lookUpInQueryString.Value.Init("QueryString"));
            providers.Add(new DateTimeLookUps().Init("DateTime"));

            return providers;
        }


    }


    public class LookUpInQueryString : LookUpBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IQueryCollection _source;

        public LookUpInQueryString(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public LookUpInQueryString Init(string name)
        {
            Name = name;
            return this;
        }

        public override string Get(string key, string format)
        {
            _source ??= _httpContextAccessor?.HttpContext?.Request.Query;
            if (_source == null) return string.Empty;
            return _source.TryGetValue(key, out var result) ? result.ToString() : string.Empty;
        }
    }

    public class DateTimeLookUps : LookUpBase
    {
        public DateTimeLookUps Init(string name)
        {
            Name = name;
            return this;
        }

        public override string Get(string key, string format)
        {
            return string.Equals(key, "Now", StringComparison.InvariantCultureIgnoreCase)
                ? DateTime.Now.ToString(format)
                : string.Empty;
        }
    }
}
