using System.Collections.Generic;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend
    {
        /// <summary>
        /// WIP v15.
        /// Later it should be built using a list of services that provide settings to the UI.
        /// - put gps coordinates in static
        /// - later get from settings
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> GetSettings()
        {
            var settings = new Dictionary<string, object>
            {
                {
                    "gps-default-coordinates", new
                    {
                        GpsLng = 9.469142499999975,
                        GpsLat = 47.17465989999999
                    }
                }
            };
            return settings;
        }
    }
}
