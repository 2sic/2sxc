using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.Code.DevTools
{
    internal class DevTools: IDevTools
    {
        public bool IsRazor { get; }
        public string RazorFileName { get; }

        public DevTools(bool isRazor, string razorFileName)
        {
            IsRazor = isRazor;
            RazorFileName = razorFileName;
        }

        private string RequireMsg(string requires, string but, string[] names) =>
            $"Partial Razor '{RazorFileName}' requires {requires} of the following parameters, but {but} were provided: " +
            string.Join(", ", (names ?? Array.Empty<string>()).Select(s => $"'{s}'"));

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
