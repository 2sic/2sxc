using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests
{
    [TestClass]
    public class CleanParamTests
    {
        [TestMethod]
        public void FloatOrNull()
        {
            // Expected Nulls
            AreEqual(null, CleanParam.DoubleOrNull(null), "null");
            AreEqual(null, CleanParam.DoubleOrNull(new object()), "null");

            // Expected Floats from other number formats
            AreEqual(0f, CleanParam.DoubleOrNull(0d), "double zero");
            AreEqual(0f, CleanParam.DoubleOrNull(0f), "float zero");
            AreEqual(1.1f, CleanParam.DoubleOrNull(1.1f), "float 1.1");
            AreEqual(1.1d, CleanParam.DoubleOrNull(1.1d), "double 1.1");

            // Expected Floats
            AreEqual(0f, CleanParam.DoubleOrNull(0), "int zero");
            AreEqual(0f, CleanParam.DoubleOrNull("0"), "string zero");
            AreEqual(0f, CleanParam.DoubleOrNull("0.0"), "string 0.0");
            AreEqual(7f, CleanParam.DoubleOrNull(7), "int 7");
            AreEqual(7f, CleanParam.DoubleOrNull("7"), "string 7");
            AreEqual(7.1d, CleanParam.DoubleOrNull("7.1"), "string 7.1");
            AreEqual(6.9d, CleanParam.DoubleOrNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void FloatOrNullWithCalculation()
        {
            // Check non-calculations
            AreEqual(0, CleanParam.DoubleOrNullWithCalculation(0));
            AreEqual(0, CleanParam.DoubleOrNullWithCalculation("0"));
            AreEqual(2, CleanParam.DoubleOrNullWithCalculation("2"));
            AreEqual(null, CleanParam.DoubleOrNullWithCalculation(""));


            // Check calculations
            AreEqual(1, CleanParam.DoubleOrNullWithCalculation("1/1"));
            AreEqual(1, CleanParam.DoubleOrNullWithCalculation("1:1"));
            AreEqual(0.5, CleanParam.DoubleOrNullWithCalculation("1/2"));
            AreEqual(0.5, CleanParam.DoubleOrNullWithCalculation("1:2"));
            AreEqual(2, CleanParam.DoubleOrNullWithCalculation("2/1"));
            AreEqual(2, CleanParam.DoubleOrNullWithCalculation("2:1"));
            AreEqual(16d / 9, CleanParam.DoubleOrNullWithCalculation("16:9"));
            AreEqual(16d / 9, CleanParam.DoubleOrNullWithCalculation("16:9"));
            AreEqual(16d / 9, CleanParam.DoubleOrNullWithCalculation("16:9"));

            // Bad calculations
            AreEqual(null, CleanParam.DoubleOrNullWithCalculation("/1"));
            AreEqual(null, CleanParam.DoubleOrNullWithCalculation(":1"));
            AreEqual(null, CleanParam.DoubleOrNullWithCalculation("1:0"));
            AreEqual(null, CleanParam.DoubleOrNullWithCalculation("0:0"));
        }

        [TestMethod]
        public void IntOrNull()
        {
            // Expected Nulls
            AreEqual(null, CleanParam.IntOrNull(null), "null");
            AreEqual(null, CleanParam.IntOrNull(new object()), "null");

            // Expected Int
            AreEqual(0, CleanParam.IntOrNull(0), "int zero");
            AreEqual(0, CleanParam.IntOrNull(0f), "float zero");
            AreEqual(0, CleanParam.IntOrNull(0d), "double zero");
            AreEqual(0, CleanParam.IntOrNull("0"), "string zero");
            AreEqual(0, CleanParam.IntOrNull("0.0"), "string 0.0");
            AreEqual(7, CleanParam.IntOrNull(7), "int 7");
            AreEqual(7, CleanParam.IntOrNull("7"), "string 7");
            AreEqual(7, CleanParam.IntOrNull("7.1"), "string 7.1");
            AreEqual(7, CleanParam.IntOrNull("6.9"), "string 6.9");
        }


        [TestMethod]
        public void IntOrZeroNull()
        {
            // Expected Nulls
            AreEqual(null, CleanParam.IntOrZeroAsNull(null), "null");
            AreEqual(null, CleanParam.IntOrZeroAsNull(new object()), "null");

            // Expected Zero Null Int
            AreEqual(null, CleanParam.IntOrZeroAsNull(0), "int zero");
            AreEqual(null, CleanParam.IntOrZeroAsNull("0"), "string zero");
            AreEqual(null, CleanParam.IntOrZeroAsNull("0.0"), "string 0.0");
            AreEqual(null, CleanParam.IntOrZeroAsNull(0f), "float zero");
            AreEqual(null, CleanParam.IntOrZeroAsNull(0d), "double zero");

            // Expected number
            AreEqual(7, CleanParam.IntOrZeroAsNull(7), "int 7");
            AreEqual(7, CleanParam.IntOrZeroAsNull("7"), "string 7");
            AreEqual(7, CleanParam.IntOrZeroAsNull("7.1"), "string 7.1");
            AreEqual(7, CleanParam.IntOrZeroAsNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void RealStringOrNull()
        {
            // Expected Nulls
            AreEqual(null, CleanParam.RealStringOrNull(null), "null");
            AreEqual(null, CleanParam.RealStringOrNull(new object()), "null");

            // Expected Zero Null Int
            AreEqual("0", CleanParam.RealStringOrNull(0), "int zero");
            AreEqual("0", CleanParam.RealStringOrNull("0"), "string zero");
            AreEqual("0.0", CleanParam.RealStringOrNull("0.0"), "string 0.0");
            AreEqual("0", CleanParam.RealStringOrNull(0f), "float zero");
            AreEqual("0", CleanParam.RealStringOrNull(0d), "double zero");

            // Expected number
            AreEqual("7", CleanParam.RealStringOrNull(7), "int 7");
            AreEqual("7", CleanParam.RealStringOrNull("7"), "string 7");
            AreEqual("7.1", CleanParam.RealStringOrNull("7.1"), "string 7.1");
            AreEqual("6.9", CleanParam.RealStringOrNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void FNearZero()
        {
            IsTrue(CleanParam.DNearZero(0));
            IsTrue(CleanParam.DNearZero(0.0001f));
            IsTrue(CleanParam.DNearZero(-0.009f));
            IsFalse(CleanParam.DNearZero(0.2f));
            IsFalse(CleanParam.DNearZero(2f));
        }

    }
}
