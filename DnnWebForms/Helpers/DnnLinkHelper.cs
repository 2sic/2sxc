using System;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent.DnnWebForms.Helpers
{
    public class DnnLinkHelper: ILinkHelper
    {
        private readonly DnnHelper Dnn;

        public DnnLinkHelper(DnnHelper dnn)
        {
            Dnn = dnn;
        }

        public string To(string requiresNamedParameters = null, string parameters = null)
        {
            // prevent incorrect use without named parameters
            if(requiresNamedParameters != null)
                throw new Exception("The Link.To can only be used with named parameters. try Link.To( parameters: \"tag=daniel&sort=up\") instead.");

            var targetPage = Dnn.Tab.TabID;

            var parametersToUse = parameters;
            return (parametersToUse == null)
                ? Dnn.Tab.FullUrl
                : DotNetNuke.Common.Globals.NavigateURL(targetPage, "", parametersToUse);

        }
    }
}