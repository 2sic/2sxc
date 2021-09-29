using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Tests.WebTests
{
    [TestClass]
    public class CleanParamTests
    {
        [TestMethod]
        public void FloatOrNull()
        {
            // Expected Nulls
            Assert.AreEqual(null, CleanParam.DoubleOrNull(null), "null");
            Assert.AreEqual(null, CleanParam.DoubleOrNull(new object()), "null");

            // Expected Floats from other number formats
            Assert.AreEqual(0f, CleanParam.DoubleOrNull(0d), "double zero");
            Assert.AreEqual(0f, CleanParam.DoubleOrNull(0f), "float zero");
            Assert.AreEqual(1.1f, CleanParam.DoubleOrNull(1.1f), "float 1.1");
            Assert.AreEqual(1.1d, CleanParam.DoubleOrNull(1.1d), "double 1.1");

            // Expected Floats
            Assert.AreEqual(0f, CleanParam.DoubleOrNull(0), "int zero");
            Assert.AreEqual(0f, CleanParam.DoubleOrNull("0"), "string zero");
            Assert.AreEqual(0f, CleanParam.DoubleOrNull("0.0"), "string 0.0");
            Assert.AreEqual(7f, CleanParam.DoubleOrNull(7), "int 7");
            Assert.AreEqual(7f, CleanParam.DoubleOrNull("7"), "string 7");
            Assert.AreEqual(7.1d, CleanParam.DoubleOrNull("7.1"), "string 7.1");
            Assert.AreEqual(6.9d, CleanParam.DoubleOrNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void FloatOrNullWithCalculation()
        {
            // Check non-calculations
            Assert.AreEqual(0, CleanParam.DoubleOrNullWithCalculation(0));
            Assert.AreEqual(0, CleanParam.DoubleOrNullWithCalculation("0"));
            Assert.AreEqual(2, CleanParam.DoubleOrNullWithCalculation("2"));
            Assert.AreEqual(null, CleanParam.DoubleOrNullWithCalculation(""));


            // Check calculations
            Assert.AreEqual(1, CleanParam.DoubleOrNullWithCalculation("1/1"));
            Assert.AreEqual(1, CleanParam.DoubleOrNullWithCalculation("1:1"));
            Assert.AreEqual(0.5, CleanParam.DoubleOrNullWithCalculation("1/2"));
            Assert.AreEqual(0.5, CleanParam.DoubleOrNullWithCalculation("1:2"));
            Assert.AreEqual(2, CleanParam.DoubleOrNullWithCalculation("2/1"));
            Assert.AreEqual(2, CleanParam.DoubleOrNullWithCalculation("2:1"));
            Assert.AreEqual(16d / 9, CleanParam.DoubleOrNullWithCalculation("16:9"));
            Assert.AreEqual(16d / 9, CleanParam.DoubleOrNullWithCalculation("16:9"));
            Assert.AreEqual(16d / 9, CleanParam.DoubleOrNullWithCalculation("16:9"));

            // Bad calculations
            Assert.AreEqual(null, CleanParam.DoubleOrNullWithCalculation("/1"));
            Assert.AreEqual(null, CleanParam.DoubleOrNullWithCalculation(":1"));
            Assert.AreEqual(null, CleanParam.DoubleOrNullWithCalculation("1:0"));
            Assert.AreEqual(null, CleanParam.DoubleOrNullWithCalculation("0:0"));
        }

        [TestMethod]
        public void IntOrNull()
        {
            // Expected Nulls
            Assert.AreEqual(null, CleanParam.IntOrNull(null), "null");
            Assert.AreEqual(null, CleanParam.IntOrNull(new object()), "null");

            // Expected Int
            Assert.AreEqual(0, CleanParam.IntOrNull(0), "int zero");
            Assert.AreEqual(0, CleanParam.IntOrNull(0f), "float zero");
            Assert.AreEqual(0, CleanParam.IntOrNull(0d), "double zero");
            Assert.AreEqual(0, CleanParam.IntOrNull("0"), "string zero");
            Assert.AreEqual(0, CleanParam.IntOrNull("0.0"), "string 0.0");
            Assert.AreEqual(7, CleanParam.IntOrNull(7), "int 7");
            Assert.AreEqual(7, CleanParam.IntOrNull("7"), "string 7");
            Assert.AreEqual(7, CleanParam.IntOrNull("7.1"), "string 7.1");
            Assert.AreEqual(7, CleanParam.IntOrNull("6.9"), "string 6.9");
        }


        [TestMethod]
        public void IntOrZeroNull()
        {
            // Expected Nulls
            Assert.AreEqual(null, CleanParam.IntOrZeroAsNull(null), "null");
            Assert.AreEqual(null, CleanParam.IntOrZeroAsNull(new object()), "null");

            // Expected Zero Null Int
            Assert.AreEqual(null, CleanParam.IntOrZeroAsNull(0), "int zero");
            Assert.AreEqual(null, CleanParam.IntOrZeroAsNull("0"), "string zero");
            Assert.AreEqual(null, CleanParam.IntOrZeroAsNull("0.0"), "string 0.0");
            Assert.AreEqual(null, CleanParam.IntOrZeroAsNull(0f), "float zero");
            Assert.AreEqual(null, CleanParam.IntOrZeroAsNull(0d), "double zero");

            // Expected number
            Assert.AreEqual(7, CleanParam.IntOrZeroAsNull(7), "int 7");
            Assert.AreEqual(7, CleanParam.IntOrZeroAsNull("7"), "string 7");
            Assert.AreEqual(7, CleanParam.IntOrZeroAsNull("7.1"), "string 7.1");
            Assert.AreEqual(7, CleanParam.IntOrZeroAsNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void RealStringOrNull()
        {
            // Expected Nulls
            Assert.AreEqual(null, CleanParam.RealStringOrNull(null), "null");
            Assert.AreEqual(null, CleanParam.RealStringOrNull(new object()), "null");

            // Expected Zero Null Int
            Assert.AreEqual("0", CleanParam.RealStringOrNull(0), "int zero");
            Assert.AreEqual("0", CleanParam.RealStringOrNull("0"), "string zero");
            Assert.AreEqual("0.0", CleanParam.RealStringOrNull("0.0"), "string 0.0");
            Assert.AreEqual("0", CleanParam.RealStringOrNull(0f), "float zero");
            Assert.AreEqual("0", CleanParam.RealStringOrNull(0d), "double zero");

            // Expected number
            Assert.AreEqual("7", CleanParam.RealStringOrNull(7), "int 7");
            Assert.AreEqual("7", CleanParam.RealStringOrNull("7"), "string 7");
            Assert.AreEqual("7.1", CleanParam.RealStringOrNull("7.1"), "string 7.1");
            Assert.AreEqual("6.9", CleanParam.RealStringOrNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void FNearZero()
        {
            Assert.IsTrue(CleanParam.DNearZero(0));
            Assert.IsTrue(CleanParam.DNearZero(0.0001f));
            Assert.IsTrue(CleanParam.DNearZero(-0.009f));
            Assert.IsFalse(CleanParam.DNearZero(0.2f));
            Assert.IsFalse(CleanParam.DNearZero(2f));
        }

    }
}
