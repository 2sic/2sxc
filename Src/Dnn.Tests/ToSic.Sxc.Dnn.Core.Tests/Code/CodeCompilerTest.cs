//using ToSic.Sxc.BuildTasks;
using ToSic.Sxc.Dnn.Core.Tests.Web;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Core.Tests.Code
{
    [TestClass]
    public class CodeCompilerTest : DnnCoreTestsBase
    {
        public Sxc.Code.CodeCompiler Compiler;
        public TestContext TestContext { get; set; }
        public HostingEnvironmentMock HostingEnvironment { get; set; }
        //public GetBuildConfig BuildConfig { get; set; }

        [TestInitialize]
        public void Setup()
        {
            HostingEnvironment = (HostingEnvironmentMock)GetService<IHostingEnvironmentWrapper>();
            HostingEnvironment.Init(TestContext);

            //BuildConfig = new GetBuildConfig();
            //var execute = BuildConfig.Execute();

            Compiler = GetService<Sxc.Code.CodeCompiler>();
        }

        [TestMethod]
        public void FileExistsInTestFiles() => Assert.IsTrue(File.Exists(HostingEnvironment.MapPath("~/placeholder.txt")));

        [TestMethod]
        public void GetAssembly_ValidFolderPath_ReturnsAssembly()
        {
            // Act
            var (assembly, errorMessage) = Compiler.GetAssembly("~/SimpleClass/");

            // Assert
            Assert.IsNotNull(assembly);
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void GetAssembly_InvalidFolderPath_ReturnsError()
        {
            // Act
            var (assembly, errorMessage) = Compiler.GetAssembly("~/path/to/invalid/folder/");

            // Assert
            Assert.IsNull(assembly);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void SimpleClass()
        {
            // Act
            var (assembly, errorMessage) = Compiler.GetAssembly("~/SimpleClass/");

            // Assert
            Assert.IsNotNull(assembly);
            Assert.IsNull(errorMessage);

            var instance = CreateInstance(assembly, "SimpleClass");
            instance.MyProperty = "Hello World";

            Assert.AreEqual("Hello World", instance.MyProperty);
            Assert.AreEqual(5, instance.Sum(2, 3));
        }

        [TestMethod]
        public void SimpleClassWithError()
        {
            // Act
            var (assembly, errorMessage) = Compiler.GetAssembly("~/SimpleClassWithError/");

            // Assert
            Assert.IsNull(assembly);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void TwoClasses()
        {
            // Act
            var (assembly, errorMessage) = Compiler.GetAssembly("~/TwoClasses/");

            // Assert
            Assert.IsNull(errorMessage);

            var instance = CreateInstance(assembly, "FirstClass");
            instance.SecondClass.MyProperty = "Hello World";

            Assert.AreEqual("Hello World", instance.SecondClass.MyProperty);
            Assert.AreEqual(5, instance.SecondClass.Sum(2, 3));

            var instance2 = instance.SecondClass.GetFirstClass();
            instance2.SecondClass.MyProperty = "Hello World";

            Assert.AreEqual("Hello World", instance2.SecondClass.MyProperty);
            Assert.AreEqual(5, instance2.SecondClass.Sum(2, 3));
        }

        [TestMethod]
        public void ThirdClassDoNotRecognizeSecondClass()
        {
            // Act
            var (assembly, errorMessage) = Compiler.GetAssembly("~/ThirdClass/");

            // Assert
            Assert.IsNull(assembly);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void ThirdClassWithError()
        {
            // Act
            var (assembly0, errorMessage0) = Compiler.GetAssembly("~/TwoClasses/");
            var secondClass = CreateInstance(assembly0, "SecondClass");
            secondClass.MyProperty = "Hello World";

            var (assembly, errorMessage) = Compiler.GetAssembly("~/ThirdClass/");

            // Assert
            Assert.IsNull(assembly);
            Assert.IsNotNull(errorMessage);
        }
    }
}
