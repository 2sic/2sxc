using System;

namespace ToSic.Sxc.Mvc.Dev
{
    public class TestIds
    {
        // Global, valid for all
        public const int PrimaryZone = 2;
        public const string DefaultLanguage = "en";
        public const int PrimaryApp = 2;

        public static InstanceId Blog = new InstanceId(2, 680, 78, 3002, new Guid("9cbcee9d-49d5-4fe0-8e74-1e20f74a5916"));

        public static InstanceId ContentOnHome = new InstanceId(2, 56, 2, 6935, new Guid("f8ae3d07-5805-4650-a46d-a047e113ab53"));

        // Token app here: http://2sexycontent.2dm.2sic/features/Tokens
        public static InstanceId Token = new InstanceId(128, 4062, 1262,9170, new Guid("584b7398-8517-4bdf-b05d-71d64b935f4f"));
        //public const string TokenAppGuid = "b255e493-63b8-4c9a-b36a-a6487730f78f";

        // public static RazorTutorial = new InstanceId();

    }
}

