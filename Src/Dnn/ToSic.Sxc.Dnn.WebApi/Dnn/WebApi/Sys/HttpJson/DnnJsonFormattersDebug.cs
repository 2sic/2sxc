using System.Net.Http.Formatting;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
internal class DnnJsonFormattersDebug
{

    internal static void DumpFormattersToLog(ILog log, string phase, MediaTypeFormatterCollection formatters)
    {
        var l = log.Fn($"dump:{phase}");
        try
        {
            l.A($"Dumping formatters ({formatters?.Count ?? -1}) at {phase}");
            if (formatters == null)
            {
                l.A("Formatters collection is NULL");
                return;
            }
            for (var i = 0; i < formatters.Count; i++)
            {
                var f = formatters[i];
                if (f == null)
                {
                    l.A($"[{i}] NULL formatter");
                    continue;
                }
                var type = f.GetType();
                var info = $"[{i}] {type.FullName}";
                // Try to unwrap common tracer pattern
                try
                {
                    var innerProp = type.GetProperty("InnerFormatter");
                    if (innerProp != null)
                        info += innerProp.GetValue(f) is MediaTypeFormatter inner
                            ? $" -> inner: {inner.GetType().FullName}"
                            : " -> inner: null";
                }
                catch { /* ignore reflection errors */ }
                l.A(info);
            }
        }
        catch (Exception ex)
        {
            l.Ex(ex);
        }
        finally
        {
            l.Done();
        }
    }

}
