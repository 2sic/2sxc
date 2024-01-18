using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Internal.Plumbing;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.PlumbingTests
{
    [TestClass]
    public class CleanParamTests
    {
        [TestMethod]
        public void FloatOrNull()
        {
            // Expected Nulls
            AreEqual(null, ParseObject.DoubleOrNull(null), "null");
            AreEqual(null, ParseObject.DoubleOrNull(new object()), "null");

            // Expected Floats from other number formats
            AreEqual(0f, ParseObject.DoubleOrNull(0d), "double zero");
            AreEqual(0f, ParseObject.DoubleOrNull(0f), "float zero");
            AreEqual(1.1f, ParseObject.DoubleOrNull(1.1f), "float 1.1");
            AreEqual(1.1d, ParseObject.DoubleOrNull(1.1d), "double 1.1");

            // Expected Floats
            AreEqual(0f, ParseObject.DoubleOrNull(0), "int zero");
            AreEqual(0f, ParseObject.DoubleOrNull("0"), "string zero");
            AreEqual(0f, ParseObject.DoubleOrNull("0.0"), "string 0.0");
            AreEqual(7f, ParseObject.DoubleOrNull(7), "int 7");
            AreEqual(7f, ParseObject.DoubleOrNull("7"), "string 7");
            AreEqual(7.1d, ParseObject.DoubleOrNull("7.1"), "string 7.1");
            AreEqual(6.9d, ParseObject.DoubleOrNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void FloatOrNullWithCalculation()
        {
            // Check non-calculations
            AreEqual(0, ParseObject.DoubleOrNullWithCalculation(0));
            AreEqual(0, ParseObject.DoubleOrNullWithCalculation("0"));
            AreEqual(2, ParseObject.DoubleOrNullWithCalculation("2"));
            AreEqual(null, ParseObject.DoubleOrNullWithCalculation(""));


            // Check calculations
            AreEqual(1, ParseObject.DoubleOrNullWithCalculation("1/1"));
            AreEqual(1, ParseObject.DoubleOrNullWithCalculation("1:1"));
            AreEqual(0.5, ParseObject.DoubleOrNullWithCalculation("1/2"));
            AreEqual(0.5, ParseObject.DoubleOrNullWithCalculation("1:2"));
            AreEqual(2, ParseObject.DoubleOrNullWithCalculation("2/1"));
            AreEqual(2, ParseObject.DoubleOrNullWithCalculation("2:1"));
            AreEqual(16d / 9, ParseObject.DoubleOrNullWithCalculation("16:9"));
            AreEqual(16d / 9, ParseObject.DoubleOrNullWithCalculation("16:9"));
            AreEqual(16d / 9, ParseObject.DoubleOrNullWithCalculation("16:9"));

            // Bad calculations
            AreEqual(null, ParseObject.DoubleOrNullWithCalculation("/1"));
            AreEqual(null, ParseObject.DoubleOrNullWithCalculation(":1"));
            AreEqual(null, ParseObject.DoubleOrNullWithCalculation("1:0"));
            AreEqual(null, ParseObject.DoubleOrNullWithCalculation("0:0"));
        }

        [TestMethod]
        public void IntOrNull()
        {
            // Expected Nulls
            AreEqual(null, ParseObject.IntOrNull(null), "null");
            AreEqual(null, ParseObject.IntOrNull(new object()), "null");

            // Expected Int
            AreEqual(0, ParseObject.IntOrNull(0), "int zero");
            AreEqual(0, ParseObject.IntOrNull(0f), "float zero");
            AreEqual(0, ParseObject.IntOrNull(0d), "double zero");
            AreEqual(0, ParseObject.IntOrNull("0"), "string zero");
            AreEqual(0, ParseObject.IntOrNull("0.0"), "string 0.0");
            AreEqual(7, ParseObject.IntOrNull(7), "int 7");
            AreEqual(7, ParseObject.IntOrNull("7"), "string 7");
            AreEqual(7, ParseObject.IntOrNull("7.1"), "string 7.1");
            AreEqual(7, ParseObject.IntOrNull("6.9"), "string 6.9");
        }


        [TestMethod]
        public void IntOrZeroNull()
        {
            // Expected Nulls
            AreEqual(null, ParseObject.IntOrZeroAsNull(null), "null");
            AreEqual(null, ParseObject.IntOrZeroAsNull(new object()), "null");

            // Expected Zero Null Int
            AreEqual(null, ParseObject.IntOrZeroAsNull(0), "int zero");
            AreEqual(null, ParseObject.IntOrZeroAsNull("0"), "string zero");
            AreEqual(null, ParseObject.IntOrZeroAsNull("0.0"), "string 0.0");
            AreEqual(null, ParseObject.IntOrZeroAsNull(0f), "float zero");
            AreEqual(null, ParseObject.IntOrZeroAsNull(0d), "double zero");

            // Expected number
            AreEqual(7, ParseObject.IntOrZeroAsNull(7), "int 7");
            AreEqual(7, ParseObject.IntOrZeroAsNull("7"), "string 7");
            AreEqual(7, ParseObject.IntOrZeroAsNull("7.1"), "string 7.1");
            AreEqual(7, ParseObject.IntOrZeroAsNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void RealStringOrNull()
        {
            // Expected Nulls
            AreEqual(null, ParseObject.RealStringOrNull(null), "null");
            AreEqual(null, ParseObject.RealStringOrNull(new object()), "null");

            // Expected Zero Null Int
            AreEqual("0", ParseObject.RealStringOrNull(0), "int zero");
            AreEqual("0", ParseObject.RealStringOrNull("0"), "string zero");
            AreEqual("0.0", ParseObject.RealStringOrNull("0.0"), "string 0.0");
            AreEqual("0", ParseObject.RealStringOrNull(0f), "float zero");
            AreEqual("0", ParseObject.RealStringOrNull(0d), "double zero");

            // Expected number
            AreEqual("7", ParseObject.RealStringOrNull(7), "int 7");
            AreEqual("7", ParseObject.RealStringOrNull("7"), "string 7");
            AreEqual("7.1", ParseObject.RealStringOrNull("7.1"), "string 7.1");
            AreEqual("6.9", ParseObject.RealStringOrNull("6.9"), "string 6.9");
        }

        [TestMethod]
        public void FNearZero()
        {
            IsTrue(ParseObject.DNearZero(0));
            IsTrue(ParseObject.DNearZero(0.0001f));
            IsTrue(ParseObject.DNearZero(-0.009f));
            IsFalse(ParseObject.DNearZero(0.2f));
            IsFalse(ParseObject.DNearZero(2f));
        }

    }
}
