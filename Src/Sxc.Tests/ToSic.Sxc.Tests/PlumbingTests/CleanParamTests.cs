using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Internal.Plumbing;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.PlumbingTests
{
    [TestClass]
    public class CleanParamTests
    {
        // Expected Nulls
        [DataRow(null, null, "null")]
        // Expected Floats from other number formats
        [DataRow(0d, 0d, "double zero")]
        [DataRow(0d, 0f, "float zero")]
        //[DataRow(1.1f, 1.1f, "float 1.1")] // edge case, conversion results in 1.1000000238418579 rounding errors
        [DataRow(1.1d, 1.1d, "double 1.1")]
        // Expected Doubles
        [DataRow(0d, 0, "int zero")]
        [DataRow(0d, "0", "string zero")]
        [DataRow(0d, "0.0", "string 0.0")]
        [DataRow(7d, 7, "int 7")]
        [DataRow(7d, "7", "string 7")]
        [DataRow(7.1d, "7.1", "string 7.1")]
        [DataRow(6.9d, "6.9", "string 6.9")]
        [TestMethod]
        public void DoubleOrNull(object expected, object data, string message)
            => AreEqual(expected, ParseObject.DoubleOrNull(data), message);

        [TestMethod]
        public void DoubleOrNullEdgeCase()
            => AreEqual(1.1f, ParseObject.DoubleOrNull(1.1f), "float 1.1");

        [TestMethod]
        public void FloatOrNullObject()
            => AreEqual(null, ParseObject.DoubleOrNull(new()), "new object");

        [TestMethod]
        public void FloatOrNullOld()
        {
            // Expected Nulls
            AreEqual(null, ParseObject.DoubleOrNull(null), "null");
            AreEqual(null, ParseObject.DoubleOrNull(new()), "null");

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

        // Check non-calculations
        [DataRow(0d, 0)]
        [DataRow(0d, "0")]
        [DataRow(2d, "2")]
        [DataRow(null, "")]
        // Check calculations
        [DataRow(1d, "1/1")]
        [DataRow(1d, "1:1")]
        [DataRow(0.5, "1/2")]
        [DataRow(0.5, "1:2")]
        [DataRow(2d, "2/1")]
        [DataRow(2d, "2:1")]
        [DataRow(16d / 9, "16:9")]
        [DataRow(16d / 9, "16/9")]
        // Bad calculations
        [DataRow(null, "/1")]
        [DataRow(null, ":1")]
        [DataRow(null, "1:0")]
        [DataRow(null, "0:0")]
        [TestMethod]
        public void DoubleOrNullWithCalculation(double? expected, object data)
            => AreEqual(expected, ParseObject.DoubleOrNullWithCalculation(data));


        [DataRow(null, null, "null")]
        [DataRow(0, 0, "int zero")]
        [DataRow(0, 0f, "float zero")]
        [DataRow(0, 0d, "double zero")]
        [DataRow(0, "0", "string zero")]
        [DataRow(0, "0.0", "string 0.0")]
        [DataRow(7, 7, "int 7")]
        [DataRow(7, "7", "string 7")]
        [DataRow(7, "7.1", "string 7.1")]
        [DataRow(7, 7.1f, "float 7.1")]
        [DataRow(7, 7.1d, "double 7.1")]
        [DataRow(7, "6.9", "string 6.9")]
        [DataRow(7, 6.9f, "float 6.9")]
        [DataRow(7, 6.9d, "double 6.9")]
        [TestMethod]
        public void IntOrNull(int? expected, object data, string message)
            => AreEqual(expected, ParseObject.IntOrNull(data), message);

        [TestMethod]
        public void IntOrNullObject()
            => AreEqual(null, ParseObject.IntOrNull(new()), "null");



        [DataRow(null, null, "null")]
        [DataRow(null, 0, "int zero")]
        [DataRow(null, "0", "string zero")]
        [DataRow(null, "0.0", "string 0.0")]
        [DataRow(null, 0f, "float zero")]
        [DataRow(null, 0d, "double zero")]
        [DataRow(7, 7, "int 7")]
        [DataRow(7, "7", "string 7")]
        [DataRow(7, "7.1", "string 7.1")]
        [DataRow(7, "6.9", "string 6.9")]
        [TestMethod]
        public void IntOrZeroNull(int? expected, object data, string message)
            => AreEqual(expected, ParseObject.IntOrZeroAsNull(data), message);

        [TestMethod]
        public void IntOrZeroNullObject()
            => AreEqual(null, ParseObject.IntOrZeroAsNull(new()), "null");


        [DataRow(null, null, "null")]
        [DataRow("0", 0, "int zero")]
        [DataRow("0", "0", "string zero")]
        [DataRow("0.0", "0.0", "string 0.0")]
        [DataRow("0", 0f, "float zero")]
        [DataRow("0", 0d, "double zero")]
        [DataRow("7", 7, "int 7")]
        [DataRow("7", "7", "string 7")]
        [DataRow("7.1", "7.1", "string 7.1")]
        [DataRow("6.9", "6.9", "string 6.9")]
        [TestMethod]
        public void RealStringOrNull(string expected, object data, string message)
            => AreEqual(expected, ParseObject.RealStringOrNull(data), message);

        [TestMethod]
        public void RealStringOrNullWithObject()
            => AreEqual(null, ParseObject.RealStringOrNull(new()), "null");


        [DataRow(true, 0)]
        [DataRow(true, 0.0001f)]
        [DataRow(true, -0.009f)]
        [DataRow(false, 0.2f)]
        [DataRow(false, 2f)]
        [TestMethod]
        public void DoubleNearZero(bool expected, double data)
            => AreEqual(expected, ParseObject.DNearZero(data));
    }
}
