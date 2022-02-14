using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {

        internal string LogHeader(string key = null)
        {
            var msg =
                      + Div("back to " + LinkTo("2sxc insights home", nameof(Help)))
                      + H1($"2sxc Insights: Log {key}")
                      + P("Status: ",
                          Strong(_logHistory.Pause ? "paused" : "running"),
                          " ",
                          LinkTo(HtmlEncode("▶"), nameof(PauseLogs), more: "toggle=false"),
                          " | ",
                          LinkTo(HtmlEncode("⏸"), nameof(PauseLogs), more: "toggle=true"),
                          $" collecting #{_logHistory.Count} of max {_logHistory.MaxCollect} (keep max {_logHistory.Size} per set, then FIFO)"
                      );
            return msg.ToString();
        }

        internal string LogHistoryOverview(LogHistory logHistory)
        {
            var msg = "";
            try
            {
                var logs = logHistory.Logs;
                msg += P($"Logs Overview: {logs.Count}\n");

                var count = 0;

                msg += Table().Id("table").Wrap(
                    HeadFields("# ↕", "Key ↕", "Count ↕", "Actions ↕"),
                    Tbody(
                        logs.OrderBy(l => l.Key)
                            .Select(log => RowFields((++count).ToString(),
                                LinkTo(log.Key, nameof(Logs), key: log.Key),
                                $"{log.Value.Count}",
                                LinkTo("flush", nameof(LogsFlush), key: log.Key))
                            )
                            .Cast<object>()
                            .ToArray())
                );
                msg += "\n\n";
                msg += JsTableSort();
            }
            catch
            {
                // ignored
            }
            return msg;
        }

        internal string LogHistory(LogHistory logHistory, string key)
        {
            var msg = "";
            //try
            //{
                if (logHistory.Logs.TryGetValue(key, out var set))
                {
                    var count = 0;
                    msg += P($"Logs Overview: {set.Count}\n");
                    msg += Table().Id("table").Wrap(
                        HeadFields("#", "Timestamp", "Key", "TopLevel Name", "Lines", "First Message"), Tbody(set
                            .Select(log => RowFields(
                                $"{++count}",
                                log.Created.ToString("O"),
                                $"{key}",
                                LinkTo(log.FullIdentifier, nameof(Logs), key: key, more: $"position={count}"),
                                $"{log.Entries.Count}",
                                log.Entries.FirstOrDefault()?.Message)
                            )
                            .ToArray<object>()));
                    msg += "\n\n";
                    msg += JsTableSort();
                }
                else
                {
                    msg += "item not found";
                }


            //}
            //catch (Exception e)
            //{
            //    // ignored
            //    throw;
            //}
            return msg;
        }

        private const string ResStart = "<span style='color: green'>= ";
        private const string ResEnd = "</span>";

        internal static string DumpTree(string title, ILog log)
        {
            var lg = new StringBuilder(H1($"{title}") + "\n\n");
            if (log.Entries.Count == 0) return "";
            lg.AppendLine("<ol>");

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
                        if (!e.WrapOpenWasClosed) lg.AppendLine("LOGGER WARNING: This logger was never closed");
                        breadcrumb.Push(e.ShortSource);
                        lg.AppendLine("<ol>");
                    }
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

            return HoverLabel(label, e.Source, "logIds")
                   + " - "
                   + HtmlEncode(e.Message)
                   + (e.Result != null
                       ? $"{ResStart}{HtmlEncode(e.Result)}{ResEnd}"
                       : string.Empty)
                   + ShowTime(e)
                   + (e.Code != null
                       ? " " + HoverLabel("C#", $"{e.Code.Path} - {e.Code.Name}() #{e.Code.Line}", "codePeek")
                       : string.Empty)
                    + "\n";
        }

        private static string ShowTime(Entry e)
        {
            if (e.Elapsed == TimeSpan.Zero) return "";
            var seconds = e.Elapsed.TotalSeconds;
            var ms = seconds * 1000;
            var time = ms < 1000 ? $"{ms:##.####}ms" : $"{seconds:##.####}s";
            return HtmlEncode($" ⌚ {time} ");
        }
    }
}
