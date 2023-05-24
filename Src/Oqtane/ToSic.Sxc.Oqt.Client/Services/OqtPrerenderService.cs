using System;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Client.Services
{
  public class OqtPrerenderService : IOqtPrerenderService
  {
        public string GetSystemHtml()
        {
            try
            {
                return SystemHtml();
            }
            catch
            {
                return string.Empty;
            }
        }

        private string SystemHtml() => $"<style> {(OqtaneVersion >= new Version(3, 0, 2) ? "body" : "app")} > div:first-of-type {{ display: block !important }} </style>";

        private Version OqtaneVersion => _oqtaneVersion ??= GetOqtaneVersion();
        private Version _oqtaneVersion;

        private static Version GetOqtaneVersion() 
            => Version.TryParse(Oqtane.Shared.Constants.Version, out var ver) ? ver : new Version(1, 0);
    }
}
