using System.Linq;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        private string Logs()
        {
            Log.Add("debug log load");
            return LogHeader("Overview") + LogHistoryOverview(_logHistory);
        }

        private string Logs(string key)
        {
            Log.Add($"debug log load for {key}");
            return LogHeader(key) + LogHistory(_logHistory, key);
        }

        private string Logs(string key, int position)
        {
            Log.Add($"debug log load for {key}/{position}");
            var msg = PageStyles() + LogHeader($"{key}[{position}]");

            if (!_logHistory.Logs.TryGetValue(key, out var set))
                return msg + $"position {position} not found in log set {key}";

            if (set.Count < position - 1)
                return msg + $"position ({position}) > count ({set.Count})";
            
            var log = set.Take(position).LastOrDefault();
            return msg + (log == null
                ? P("log is null").ToString()
                : DumpTree($"Log for {key}[{position}]", log));
        }

        private string PauseLogs(bool pause)
        {
            Log.Add($"pause log {pause}");
            _logHistory.Pause = pause;
            return $"pause set to {pause}";
        }

        private string LogsFlush(string key)
        {
            Log.Add($"flush log for {key}");
            _logHistory.Flush(key);
            return $"flushed log history for {key}";
        }
    }
}
