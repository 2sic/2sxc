using System;
using DotNetNuke.Services.Exceptions;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnRenderingHelper : RenderingHelper
    {
        /// <summary>
        /// Constructor for IoC
        /// You must always call Init afterwards
        /// </summary>
        /// <param name="http"></param>
        public DnnRenderingHelper(IHttp http) : base( http,"Dnn.Render") { }

        /// <summary>
        /// DNN specific implementation to log errors to the DNN event log
        /// </summary>
        /// <param name="ex"></param>
        protected override void LogToEnvironmentExceptions(Exception ex) => Exceptions.LogException(ex);
    }
}