using System;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code
{
    internal class DevTools: ServiceBase, IDevTools
    {
        public bool IsRazor { get; }
        public string RazorFileName { get; }

        public DevTools(bool isRazor, string razorFileName, ILog parentLog): base($"{Constants.SxcLogName}.DevTls")
        {
            IsRazor = isRazor;
            RazorFileName = razorFileName;
            this.LinkLog(parentLog);
        }

        private string RequireMsg(string requires, string but, string[] names) =>
            $"Partial Razor '{RazorFileName}' requires {requires} of the following parameters, but {but} were provided: " +
            string.Join(", ", (names ?? Array.Empty<string>()).Select(s => $"'{s}'"));

        public void Debug(object target, string noParamOrder = Protector, bool debug = true)
        {
            var l = Log.Fn($"{nameof(target)}: '{target?.GetType()}', {nameof(debug)}: {debug}");
            Protect(noParamOrder);
            if (!(target is ICanDebug canDebug))
                throw new ArgumentException($"Can't enable debug on {nameof(target)} as it doesn't support {nameof(ICanDebug)}");
            canDebug.Debug = debug;
            l.Done();
        }

        //public bool HasAll(params string[] names)
        //{
        //    if (names == null || names.Length == 0) return true;
        //    return names.All(n => _paramsDictionary.ContainsKey(n));
        //}

        //public bool HasAny(params string[] names)
        //{
        //    if (names == null || names.Length == 0) return true;
        //    return names.Any(n => _paramsDictionary.ContainsKey(n));
        //}

        //public string RequireAny(params string[] names)
        //{
        //    if (HasAny(names)) return null;
        //    throw new ArgumentException(RequireMsg("one or more", "none", names));
        //}
        //public string RequireAll(params string[] names)
        //{
        //    if (HasAll(names)) return null;
        //    throw new ArgumentException(RequireMsg("all", "not all", names));
        //}

    }
}
