using System;
using DotNetNuke.Services.Exceptions;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnRenderingHelper : RenderingHelper
    {
        /// <summary>
        /// Constructor for IoC
        /// You must always call Init afterwards
        /// </summary>
        public DnnRenderingHelper(ILinkPaths linkPaths) : base(linkPaths, "Dnn.Render") { }

        /// <summary>
        /// DNN specific implementation to log errors to the DNN event log
        /// </summary>
        /// <param name="ex"></param>
        protected override void LogToEnvironmentExceptions(Exception ex) => Exceptions.LogException(ex);
    }
}