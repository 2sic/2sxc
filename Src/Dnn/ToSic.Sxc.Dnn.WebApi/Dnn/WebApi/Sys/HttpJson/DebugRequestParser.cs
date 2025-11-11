using System.Diagnostics;
using System.Web;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

// Helper for parsing debug decision from querystring, headers and cookies.
internal class DebugRequestParser(ILog parentLog) : HelperBase(parentLog, "Dnn.DbRqPr")
{
    private const string Debug = "debug";

    // Determines whether tracing is enabled for the given HttpContext.
    // It follows the same precedence as previous logic: query string, headers, cookie.
    internal bool IsDebugEnabled()
    {
        var l = Log.Fn<bool>("parse debug state");
        try
        {
            // If a debugger is attached in DEBUG builds, always trace
#if DEBUG
            if (Debugger.IsAttached)
            {
                l.A("Debugger attached -> debug = true");
                return l.ReturnTrue("attached");
            }
#endif
            var ctx = HttpContext.Current; // may be null outside request scope
            if (ctx == null)
            {
                l.A("No HttpContext -> debug = false");
                return l.ReturnFalse("no-context");
            }

            var hasExplicitDecision = false;
            var explicitEnable = false;

            // 1) Query string
            try
            {
                var req = ctx.Request;
                var hasKey = req?.QueryString.AllKeys?.Contains(Debug) == true;
                if (hasKey)
                {
                    var qVal = req!.QueryString[Debug]; // may be null if just ?debug
                    l.A($"query:'{qVal}'");
                    if (qVal == null)
                    {
                        hasExplicitDecision = true;
                        explicitEnable = true; // presence without value means on
                    }
                    else if (IsTruthy(qVal))
                    {
                        hasExplicitDecision = true;
                        explicitEnable = true;
                    }
                    else if (IsFalsey(qVal))
                    {
                        hasExplicitDecision = true;
                        explicitEnable = false;
                    }
                }
            }
            catch (Exception ex) { l.Ex(ex); }

            // 2) Headers check only if not already decided
            try
            {
                if (!hasExplicitDecision)
                {
                    var req = ctx.Request;
                    var headers = req?.Headers;
                    if (headers != null)
                    {
                        var headerNames = new[] { Debug };
                        foreach (var hName in headerNames)
                        {
                            var hVal = headers[hName];
                            l.A($"header {hName}='{hVal}'");

                            if (hVal == null)
                                continue;

                            if (IsTruthy(hVal))
                            {
                                hasExplicitDecision = true;
                                explicitEnable = true;
                                break;
                            }

                            if (IsFalsey(hVal))
                            {
                                hasExplicitDecision = true;
                                explicitEnable = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                l.Ex(ex);
            }

            // Read existing cookie value (used for fallback or if we need to update it later)
            var cookieIsTruthy = false;
            HttpCookie existingCookie = null;
            try
            {
                existingCookie = ctx.Request?.Cookies?[Debug];
                if (existingCookie != null)
                {
                    l.A($"cookie:'{existingCookie.Value}'");
                    if (IsTruthy(existingCookie.Value))
                        cookieIsTruthy = true;
                }
            }
            catch (Exception ex)
            {
                l.Ex(ex);
            }

            // Apply explicit decision to cookie
            try
            {
                if (hasExplicitDecision)
                {
                    if (explicitEnable)
                    {
                        // set/update session cookie (in-memory for this browser session)
                        var cookie = new HttpCookie(Debug, "true")
                        {
                            // no Expires -> session cookie
                            HttpOnly = false // allow JS if needed
                        };
                        ctx.Response?.Cookies?.Set(cookie);
                        l.A("set cookie=true");
                    }
                    else
                    {
                        // expire cookie
                        if (existingCookie != null)
                        {
                            existingCookie.Value = "false";
                            existingCookie.Expires = DateTime.UtcNow.AddDays(-1);
                            ctx.Response?.Cookies?.Set(existingCookie);
                            l.A("expire cookie (existing)");
                        }
                        else
                        {
                            var cookie = new HttpCookie(Debug, "false") { Expires = DateTime.UtcNow.AddDays(-1) };
                            ctx.Response?.Cookies?.Add(cookie);
                            l.A("expire cookie (new)");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                l.Ex(ex);
            }

            // Final decision
            if (hasExplicitDecision)
            {
                l.A($"explicit = {explicitEnable}");
                return l.Return(explicitEnable, explicitEnable ? "explicit-on" : "explicit-off");
            }

            // Fallback to cookie state
            l.A($"fallback cookie = {cookieIsTruthy}");
            return l.Return(cookieIsTruthy, cookieIsTruthy ? "cookie-on" : "cookie-off");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("error");
        }
    }

    // For tests, these are internal so they can be validated.
    private static bool IsTruthy(string value)
    {
        if (value == null)
            return false;

        var v = value.Trim().ToLowerInvariant();
        return v.Length == 0
               || v is "true" or "1" or "yes" or "on";
    }

    private static bool IsFalsey(string value)
    {
        if (value == null)
            return false;

        var v = value.Trim().ToLowerInvariant();
        return v is "false" or "0" or "no" or "off";
    }
}