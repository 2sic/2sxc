using System.Linq;
using ToSic.Eav.Logging;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {

        public string Logs()
        {
            ThrowIfNotSuperUser();
            Log.Add("debug log load");
            return LogHeader("Overview") + LogHistoryOverview();
        }

        public string Logs(string key)
        {
            ThrowIfNotSuperUser();
            Log.Add($"debug log load for {key}");
            return LogHeader(key) + LogHistory(key);
        }

        public string Logs(string key, int position)
        {
            ThrowIfNotSuperUser();
            Log.Add($"debug log load for {key}/{position}");
            var msg = PageStyles() + LogHeader();
            if (History.Logs.TryGetValue(key, out var set))
            {
                if (set.Count >= position - 1)
                {
                    var log = set.Take(position).LastOrDefault();
                    msg += log == null
                        ? P("log is null").ToString()
                        : DumpTree($"Log for {key}[{position}]", log);
                }
                else
                    msg += $"position ({position}) > count ({set.Count})";
            }
            else
                msg += $"position {position} not found in log set {key}";

            //msg += PageStyles();
            return msg;
        }

        public string Logs(bool pause)
        {
            ThrowIfNotSuperUser();
            Log.Add($"pause log {pause}");
            History.Pause = pause;
            return $"pause set to {pause}";
        }

        public string LogsFlush(string key)
        {
            ThrowIfNotSuperUser();
            Log.Add($"flush log for {key}");
            History.Flush(key);
            return $"flushed log history for {key}";
        }
    }
}
