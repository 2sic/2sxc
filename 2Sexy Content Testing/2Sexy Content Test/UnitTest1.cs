using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.SexyContent;

namespace _2Sexy_Content_Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			SexyContentModuleUpgrade.UpgradeModule("07.00.00");
		}
	}
}
