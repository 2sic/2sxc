using System.Linq;
using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {

        [HttpGet]
        public string Logs()
        {
            ThrowIfNotSuperuser();
            Log.Add("debug log load");
            return LogHeader("Overview") + LogHistoryOverview();
        }

        [HttpGet]
        public string Logs(string key)
        {
            ThrowIfNotSuperuser();
            Log.Add($"debug log load for {key}");
            return LogHeader(key) + LogHistory(key);
        }

        [HttpGet]
        public string Logs(string key, int position)
        {
            ThrowIfNotSuperuser();
            Log.Add($"debug log load for {key}/{position}");
            var msg = LogHeader();
            if (History.Logs.TryGetValue(key, out var set))
            {
                if (set.Count >= position - 1)
                {
                    var log = set.Take(position).LastOrDefault();
                    msg += log == null
                        ? p("log is null")
                        : FormatLog($"Log for {key}[{position}]", log);
                }
                else
                    msg += $"position ({position}) > count ({set.Count})";
            }
            else
                msg += $"position {position} not found in log set {key}";
            return msg;
        }

        [HttpGet]
        public string Logs(bool pause)
        {
            ThrowIfNotSuperuser();
            Log.Add($"pause log {pause}");
            History.Pause = pause;
            return $"pause set to {pause}";
        }

        [HttpGet]
        public string LogsFlush(string key)
        {
            ThrowIfNotSuperuser();
            Log.Add($"flush log for {key}");
            History.Flush(key);
            return $"flushed log history for {key}";
        }

        private static string LogHeader(string key = null)
        {
            var msg = h1($"Log {key}")
                      + p($"Status: {(History.Pause ? "paused" : "running")} collecting #{History.Count} of max {History.MaxCollect} (keep max {History.Size} per set, then FIFO) - "
                          + a("change", $"logs?pause={!History.Pause}")
                          + " (pause to see details of the log)\n");
            return msg;
        }

        private static string LogHistoryOverview()
        {
            var msg = "";
            try
            {
                var logs = History.Logs;
                msg += p($"Logs Overview: {logs.Count}\n");

                msg += "<table id='table'><thead>"
                       + tr(new[] {"#", "Key", "Count", "Actions"}, true)
                       + "</thead>"
                       + "<tbody>";
                var count = 0;
                foreach (var log in logs.OrderBy(l => l.Key))
                {
                    msg = msg + tr(new[]
                    {
                        (++count).ToString(),
                        a(log.Key, $"logs?key={log.Key}"),
                        $"{log.Value.Count}",
                        a("flush", $"logsflush?key={log.Key}")
                    });
                }
                msg += "</tbody>";
                msg += "</table>";
                msg += "\n\n";
                msg += JsTableSort();
            }
            catch
            {
                // ignored
            }
            return msg;
        }

        private static string LogHistory(string key)
        {
            var msg = "";
            try
            {
                if(History.Logs.TryGetValue(key, out var set))
                {
                    msg += p($"Logs Overview: {set.Count}\n");
                    msg += "<table id='table'><thead>"
                           + tr(new[] {"#", "Timestamp", "Key", "TopLevel Name", "Lines", "First Message"}, true)
                           + "</thead>"
                           + "<tbody>";
                    var count = 0;
                    foreach (var log in set)
                    {
                        count++;
                        msg = msg + tr(new[]
                        {
                            $"{count}",
                            log.Created.ToString("O"),
                            $"{key}",
                            a(log.FullIdentifier, $"logs?key={key}&position={count}"),
                            $"{log.Entries.Count}",
                            log.Entries.FirstOrDefault()?.Message
                        });
                    }
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "\n\n";
                    msg += JsTableSort();                    
                }
                else
                {
                    msg += "item not found";
                }


            }
            catch
            {
                // ignored
            }
            return msg;
        }

        private static string FormatLog(string title, Log log)
            => ToBr(log.Dump(" - ", h1($"{title}") + "\n",
                "end of log")).Replace("⋮", "&vellip;");
    }
}