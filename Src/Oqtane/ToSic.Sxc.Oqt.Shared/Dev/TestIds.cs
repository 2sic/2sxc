using System;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Oqt.Shared.Dev
{
    public class TestIds
    {
        public static bool Dev4Spm = false;

        // Global, valid for all
        public static int PrimaryZone => Dev4Spm ? 2 : 2;
        public static int PrimaryApp => Dev4Spm ? 999 : 2;

        public static InstanceId Blog => Dev4Spm
            ? new InstanceId(2, 55, 4, 412, new Guid("b9bc0e05-2dc1-451b-a185-bd22a03e0952"))
            : new InstanceId(2, 680, 78, 3002, new Guid("9cbcee9d-49d5-4fe0-8e74-1e20f74a5916"));

        public static InstanceId ContentOnHome => Dev4Spm
            ? new InstanceId(2, 56, 2, 6935, new Guid("f8ae3d07-5805-4650-a46d-a047e113ab53"))
            : new InstanceId(2, 56, 2, 6935, new Guid("f8ae3d07-5805-4650-a46d-a047e113ab53"));

        // Token app here: http://2sexycontent.2dm.2sic/features/Tokens
        public static InstanceId Token => Dev4Spm
            ? new InstanceId(2, 92, 18, 428, new Guid("7e4dfd75-4158-44cd-a08f-d3078f9bd3ff"))
            : new InstanceId(128, 4062, 1262,9170, new Guid("584b7398-8517-4bdf-b05d-71d64b935f4f"));

        // Slider / Swiper App
        public static InstanceId Swiper => Dev4Spm
            ? new InstanceId(138, 4216, 1318, 9476, new Guid("17c84277-d427-4df7-9748-5e8fcbb402f2")) // todo: SPM not correct yet
            : new InstanceId(138, 4216, 1318, 9476, new Guid("17c84277-d427-4df7-9748-5e8fcbb402f2"));

        public static List<InstanceId> FakeDb => new List<InstanceId>
        {
            Blog,
            ContentOnHome,
            Token,
            Swiper,
        };

        public static InstanceId FindInstance(int containerId) => FakeDb.FirstOrDefault(i => i.Container == containerId);
    }
}

