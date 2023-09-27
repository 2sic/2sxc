using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Core.Tests.Web
{
    /// <summary>
    /// mock HostingEnvironment
    /// </summary>
    public class HostingEnvironmentMock : IHostingEnvironmentWrapper
    {
        public TestContext TestContext { get; set; }


        private string TestStorageRoot => TestContext.DeploymentDirectory + "\\..\\..\\..\\Dnn.Tests\\ToSic.Sxc.Dnn.Core.Tests\\.tests-files\\";

        public void Init(TestContext testContext)
        {
            TestContext = testContext;
        }

        public string MapPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~/") || virtualPath.StartsWith("./"))
                virtualPath = virtualPath.Substring(2);

            if (virtualPath.StartsWith("/"))
                virtualPath = virtualPath.Substring(1);

            //if (!virtualPath.EndsWith("/"))
            //    virtualPath += "/";

            virtualPath = virtualPath.Replace("/", "\\");

            return Path.Combine(TestStorageRoot, virtualPath);
        }
    }
}
