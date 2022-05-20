using System;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Services
{
    public class FeaturesService: HasLog, IFeaturesService, ICanDebug
    {
        public FeaturesService(Eav.Configuration.IFeaturesInternal root) : base($"{Constants.SxcLogName}.FeatSv")
            => _root = root;

        private readonly Eav.Configuration.IFeaturesInternal _root;

        public bool IsEnabled(params string[] nameIds)
        {
            var result = _root.IsEnabled(nameIds);
            if (!Debug) return result;
            var wrapLog = Log.Fn<bool>(string.Join(",", nameIds ?? Array.Empty<string>()));
            return wrapLog.Return(result, $"{result}");
        }

        //public bool Valid => _root.Valid;

        public bool Debug { get; set; }
    }
}
