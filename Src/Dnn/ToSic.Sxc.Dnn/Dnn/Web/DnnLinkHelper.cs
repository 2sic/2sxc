using System;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// The DNN implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class DnnLinkHelper: ILinkHelper
    {
        private IDnnContext _dnn;

        public DnnLinkHelper() { }

        public DnnLinkHelper Init(IDnnContext dnn)
        {
            _dnn = dnn;
            return this;
        }

        /// <inheritdoc />
        public string To(string requiresNamedParameters = null, int? pageId = null, string parameters = null)
        {
            // prevent incorrect use without named parameters
            if(requiresNamedParameters != null)
                throw new Exception("The Link.To can only be used with named parameters. try Link.To( parameters: \"tag=daniel&sort=up\") instead.");

            var targetPage = pageId ?? _dnn.Tab.TabID;

            var parametersToUse = parameters;
            return parametersToUse == null
                ? _dnn.Tab.FullUrl
                : DotNetNuke.Common.Globals.NavigateURL(targetPage, "", parametersToUse);

        }

        /// <inheritdoc />
        public string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.Ordinal));

        }

        public string Api(string noParameterOrder = Eav.Constants.RandomProtectionParameter, string path = null)
        {
            // TODO: STV
            // 1. if path starts with / remove that
            // 2. if it starts with "app/" or "api/" or "some-edition/api" or "app/some-edition/api" should always behave the same
            // 3. should then return a full link (without domain) to the app endpoint
            // Make sure to access an object or code which already does this work, like the stuff which generates the in-page js context or something
            throw new NotImplementedException();
        }
    }
}