using System.Diagnostics;
using System.Web;
using DotNetNuke.Entities.Controllers;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

// Helper for parsing debug decision from querystring and host-setting (no headers, no empty values).
internal class GlobalDebugParser(ILog parentLog) : HelperBase(parentLog, "Dnn.DbRqPr")
{
    private const string Debug = "debug";

    // HostSetting key (DNN Host / SuperUser scope) - avoids web.config edits/restarts
    private const string HostSettingForceDebugKey = "Sxc:ForceDebug";

    // Global state cached in-memory for fast checks (not user-bound)
    private static bool? _globalDebug;
    private static readonly object _globalLock = new object();

    // Determines whether tracing is enabled for the given HttpContext.
    // Precedence: explicit query toggle -> cached host-setting -> default(false)
    internal bool IsDebugEnabled()
    {
        var l = Log.Fn<bool>("parse debug state");
        try
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                l.A("Debugger attached -> debug = true");
                return l.ReturnTrue("attached");
            }
#endif
            var ctx = HttpContext.Current; // may be null outside request scope

            // 1) Query string explicit toggle (requires value true/false; no empty value supported)
            try
            {
                if (ctx?.Request != null)
                {
                    var req = ctx.Request;
                    var hasKey = req.QueryString.AllKeys?.Contains(Debug) == true;
                    if (hasKey)
                    {
                        var qVal = req.QueryString[Debug];
                        l.A($"query:'{qVal}'");

                        if (qVal != null)
                        {
                            if (IsTruthy(qVal))
                            {
                                SetGlobalDebug(true, out var persistedOk);
                                l.A($"explicit query toggle on; host-setting-persist={(persistedOk ? "ok" : "fail")}");
                                return l.ReturnTrue("explicit-on");
                            }

                            if (IsFalsey(qVal))
                            {
                                SetGlobalDebug(false, out var persistedOk);
                                l.A($"explicit query toggle off; host-setting-persist={(persistedOk ? "ok" : "fail")}");
                                return l.ReturnFalse("explicit-off");
                            }

                            l.A("query value invalid -> ignore");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                l.Ex(ex);
            }

            // 2) Fallback to global cached host-setting
            try
            {
                var forced = GetGlobalDebug();
                if (forced.HasValue)
                {
                    l.A($"global forced = {forced.Value}");
                    return l.Return(forced.Value, forced.Value ? "global-on" : "global-off");
                }
            }
            catch (Exception ex)
            {
                l.Ex(ex);
            }

            l.A("default = false");
            return l.ReturnFalse("default-off");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("error");
        }
    }

    // Retrieve forced debug from cache or HostSettings
    private static bool? GetGlobalDebug()
    {
        if (_globalDebug.HasValue)
            return _globalDebug.Value;

        lock (_globalLock)
        {
            if (_globalDebug.HasValue)
                return _globalDebug.Value;

            try
            {
                var raw = HostController.Instance.GetString(HostSettingForceDebugKey, null);
                if (raw == null)
                {
                    _globalDebug = null; // unset
                }
                else if (IsTruthy(raw))
                {
                    _globalDebug = true;
                }
                else if (IsFalsey(raw))
                {
                    _globalDebug = false;
                }
                else
                {
                    _globalDebug = null; // unknown -> ignore
                }
            }
            catch
            {
                _globalDebug = null;
            }
            return _globalDebug;
        }
    }

    // Set global forced debug & persist in HostSettings; returns persistence success
    private static void SetGlobalDebug(bool enable, out bool persistedOk)
    {
        lock (_globalLock)
        {
            _globalDebug = enable;
            persistedOk = TryPersistHostSetting(enable);
        }
    }

    private static bool TryPersistHostSetting(bool enable)
    {
        try
        {
            var value = enable ? "true" : "false";
            HostController.Instance.Update(HostSettingForceDebugKey, value, clearCache: true);
            return true;
        }
        catch
        {
            return false; // keep in-memory state
        }
    }

    private static bool IsTruthy(string value)
    {
        if (value == null) return false;
        var v = value.Trim().ToLowerInvariant();
        return v.Length == 0 || v is "true" or "1" or "yes" or "on";
    }

    private static bool IsFalsey(string value)
    {
        if (value == null) return false;
        var v = value.Trim().ToLowerInvariant();
        return v is "false" or "0" or "no" or "off";
    }
}