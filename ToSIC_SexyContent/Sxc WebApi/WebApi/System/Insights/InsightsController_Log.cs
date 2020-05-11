using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

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
                        : DumpTree($"Log for {key}[{position}]", log);
                }
                else
                    msg += $"position ({position}) > count ({set.Count})";
            }
            else
                msg += $"position {position} not found in log set {key}";

            msg += InsightsHtml.PageStyles();
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
            var msg = div("back to " + a("2sxc insights home", "./help"))
                      + h1($"2sxc Insights: Log {key}")
                      + p(
                          $"Status: {(History.Pause ? "paused" : "running")} collecting #{History.Count} of max {History.MaxCollect} (keep max {History.Size} per set, then FIFO) - "
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

        private const string ResStartPlaceholder = "*resStart*";
        private const string ResEndPlaceholder = "*resEnd*";
        private const string ResStart = "<span style='color: green'>= ";
        private const string ResEnd = "</span>";

        private const string CallerPrefixPlaceholder = "*clrP*";
        private const string CallerSuffixPlaceholder = "*clrS";
        private const string CallerPrefix = " <span style='color: blue' title='";
        private const string CallerSuffix = "'>C#</span>";

        private static string FormatLog(string title, ILog log)
        {
            var dump = log.Dump(" - ", 
                "",
                "end of log",
                ResStartPlaceholder, 
                ResEndPlaceholder,
                withCaller: true,
                callStart: CallerPrefixPlaceholder,
                callEnd: CallerSuffixPlaceholder
                );
            var htmlEnc = h1($"{title}") + "\n" + InsightsHtml.HtmlEncode(dump);
            htmlEnc = htmlEnc
                .Replace(ResStartPlaceholder, ResStart)
                .Replace(ResEndPlaceholder, ResEnd)
                .Replace(CallerPrefixPlaceholder, CallerPrefix)
                .Replace(CallerSuffixPlaceholder, CallerSuffix);

            return ToBr(htmlEnc);
        }

        private static string DumpTree(string title, ILog log)
        {
            var lg = new StringBuilder(h1($"{title}") + "\n\n");
            if (log.Entries.Count == 0) return "";
            lg .AppendLine("<ol>");

            var breadcrumb = new Stack<string>();

            foreach (var e in log.Entries)
            {
                // a wrap-close should happen before adding a line, since it must go up a level
                if (e.WrapClose)
                {
                    lg.AppendLine("</ol></li>");
                    if (breadcrumb.Count > 0) breadcrumb.Pop();
                }
                else
                {
                    lg.AppendLine("<li>");
                    lg.AppendLine(TreeDumpOneLine(e, breadcrumb.Count > 0 ? breadcrumb.Peek() : ""));
                    if (e.WrapOpen)
                    {
                        breadcrumb.Push(e.ShortSource);
                        lg.AppendLine("<ol>");
                    }
                    //else if (e.WrapClose)
                    //{
                    //    lg.AppendLine("</ol></li>");
                    //    if (breadcrumb.Count > 0) breadcrumb.Pop();
                    //}
                    else lg.AppendLine("</li>");
                }
            }

            lg.Append("</ol>");
            lg.Append("end of log");
            return lg.ToString();
        }

        private static string TreeDumpOneLine(Entry e, string parentName)
        {
            // if it's just a close, only repeat the result
            if (e.WrapClose)
                return $"{ResStart}{e.Result}{ResEnd}";

            #region find perfect Label

            var label = e.Source;
            if (!string.IsNullOrEmpty(parentName) && !string.IsNullOrEmpty(e.Source))
            {
                var foundParent = e.Source?.IndexOf(parentName) ?? 0;
                if (foundParent > 0)
                {
                    var cut = foundParent + parentName.Length;
                    if (!(label.Length > cut))
                        cut = foundParent;
                    label = e.Source.Substring(cut);
                }
            }

            #endregion

            return InsightsHtml.HoverLabel(label, e.Source, "logIds")
                   + " - "
                   + InsightsHtml.HtmlEncode( e.Message)
                   + (e.Result != null 
                       ? ResStart + e.Result + ResEnd 
                       : string.Empty)
                   + ShowTime(e)
                   + (e.Code != null
                       ? " " + InsightsHtml.HoverLabel("C#", $"{e.Code.Path} - {e.Code.Name}() #{e.Code.Line}", "codePeek")
                       : string.Empty)
                    + "\n";
        }

        private static string ShowTime(Entry e)
        {
            if (e.Elapsed == TimeSpan.Zero) return "";
            var seconds = e.Elapsed.TotalSeconds;
            var ms = seconds * 1000;
            var time = ms < 1000 ? $"{ms}ms" : $"{seconds}s";
            return InsightsHtml.HtmlEncode($" ⌚ {time} ");
        }
    }
}