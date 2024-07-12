using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    [TestClass]
    public class ResizeParamsBestWh: LinkImageTestBase
    {


        const int s = 1440; // standard value, divisible by 1/2/3/4/6/12

        [TestMethod]
        public void FigureOutBestWidthAndHeight()
        {
            // All Zero and Nulls
            IsTrue(TestBestWH(0, 0), "Null everything");
            IsTrue(TestBestWH(0, 0, 0), "Null everything");
            IsTrue(TestBestWH(0, 0, 0, 0), "Null everything");
            IsTrue(TestBestWH(0, 0, 0, 0, 0), "Null everything");
            IsTrue(TestBestWH(0, 0, 0, 0, 0, 0), "Null everything");

            // Single Aspect is set
            IsTrue(TestBestWH(s, 0, width: s), "W only");
            IsTrue(TestBestWH(0, s, height: s), "H only");
            IsTrue(TestBestWH(0, 0, factor: 7), "factor only");
            IsTrue(TestBestWH(0, 0, ar: 7), "ar only");

            // H / W
            IsTrue(TestBestWH(s, s, width: s, height: s), "W & H only");
        }


        // W / Factor - factor should apply now 13.03
        [DataRow(s, 0, s, 1, "W/F only")]
        [DataRow(s * .5, 0, s, .5, "W/F only")]
        [DataRow(s * 2, 0, s, 2, "W/F only")]
        [DataRow(s * 1.5, 0, s, 1.5, "W/F only")]
        [DataTestMethod]
        public void FigureOutBestWidthAndHeight_Width(double expW, int expH, int w, double f, string name) 
            => IsTrue(TestBestWH((int)expW, expH, width: w, factor: f), name);

        // H / W / Factor - factor should  apply now 13.03
        [DataRow(s, s, 1, "W/H/F")]
        [DataRow(s * 0.5, s, .5, "W/H/F")]
        [DataRow(s * 2, s, 2, "W/H/F")]
        [DataRow(s * 1.5, s, 1.5, "W/H/F")]
        [DataTestMethod]
        public void FigureOutBestWidthAndHeight_Square(double exp, int x, double f, string name) 
            => IsTrue(TestBestWH((int)exp, (int)exp, width: x, height: x, factor: f), name);


        // W / Factor - factor should apply now 13.03
        [DataRow(s, 0, s, 1, "W/F only")]
        [DataRow(s * .5, 0, s, .5, "W/F only")]
        [DataRow(s * 2, 0, s, 2, "W/F only")]
        [DataRow(s * 1.5, 0, s, 1.5, "W/F only")]
        [DataTestMethod]
        public void FigureOutBestWidthAndHeight_Height(double expH, int expW, int h, double f, string name) 
            => IsTrue(TestBestWH(expW, (int)expH, height: h, factor: f), "H/F only");

        [TestMethod]
        public void FigureOutBestWidthAndHeight_SettingsWH()
        {
            IsTrue(TestBestWH(s, 0, settings: ToDyn(new { Width = s })), "W only");
            IsTrue(TestBestWH(0, s, settings: ToDyn(new { Height = s })), "H only");
            IsTrue(TestBestWH(0, 0, settings: ToDyn(new { })), "No params");
            IsTrue(TestBestWH(s, s, settings: ToDyn(new { Width = s, Height = s })), "W/H params");
        }


        [TestMethod]
        public void FigureOutBestWH_SettingsWithNeutralizer()
        {
            IsTrue(TestBestWH(0, 0, 0, null, null, null, ToDyn(new { Width = 700 })));
        }

        [DataRow(0, 1, "Factor 0, result 1")]
        [DataRow(1, 1, "Factor 1, result 1")]
        [DataRow(2, 2, "Factor 2, result 2")]
        [DataRow(0.5, 0.5, "Factor / result 0.5")]
        [DataTestMethod]
        public void FigureOutBestWidthAndHeight_SettingsWHF(double factor, double fResult, string name)
        {
            Console.WriteLine("Run with Factor: " + factor + $"; start-value is {s}");
            // Factor 1
            IsTrue(TestBestWH((int)(fResult * s), 0, factor: factor, settings: ToDyn(new { Width = s })), $"Calc W with f:{factor}");
            IsTrue(TestBestWH(0, (int)(fResult * s), factor: factor, settings: ToDyn(new { Height = s })), $"Calc H with f:{factor}");
            IsTrue(TestBestWH(0, 0, factor: factor, settings: ToDyn(new { })), $"No params - f shouldn't have an effect f:{factor}");
            IsTrue(TestBestWH((int)(fResult * s), (int)(fResult * s), factor: factor, settings: ToDyn(new { Width = s, Height = s })), $"Calc W and H in settings with f:{factor}");
        }


        internal static bool TestBestWH(int expW, int expH, object width = null, object height = null, object factor = null, object ar = null, ICanGetByName settings = null)
        {
            // Get a new linker for each test run
            var paramMerger = new ResizeParamMerger(null);
            var dimGen = new ResizeDimensionGenerator();

            var resizeSettings = paramMerger.BuildCoreSettings(new(null), width, height, factor, ar, null, settings);
            var t1 = dimGen.ResizeDimensions(resizeSettings, resizeSettings.Find(SrcSetType.Img, true, null));
            var okW = expW.Equals(t1.Width);
            var okH = expH.Equals(t1.Height);
            var ok = okW && okH;
            Console.WriteLine((ok ? "ok" : "error") + "; W:" + t1.Width + (okW ? "==": "!=") + expW + "; H:" + t1.Width + (okH ? "==" : "!=") + expH);
            return ok;
        }
    }
}
