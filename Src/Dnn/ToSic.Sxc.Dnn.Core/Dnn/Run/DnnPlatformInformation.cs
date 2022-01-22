using System;
using System.Data.SqlClient;
using DotNetNuke.Application;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// DNN implementation of the Fingerprinting system for extra security.
    /// </summary>
    [PrivateApi("Hide implementation")]
    public class DnnPlatformInformation: PlatformInformationBase
    {
        public override string Name => "Dnn";

        public override Version Version => DotNetNukeContext.Current.Application.Version;

        public override string Identity => DotNetNuke.Entities.Host.Host.GUID;

        //public override string DbConnection { 
        //    get
        //    {
        //        {
        //            var connBuilder = new SqlConnectionStringBuilder
        //            {
        //                ConnectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString()
        //            };
        //            return connBuilder.InitialCatalog;
        //        }
        //    }
        //}
    }
}
