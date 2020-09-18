using System;

namespace ToSic.Sxc.Mvc.Dev
{
    public class InstanceId
    {
        public InstanceId(int t, int p, int a, int c, Guid b)
        {
            Zone = t;
            Page = p;
            App = a;
            Container = c;
            Block = b;
        }

        public int Zone;
        public int Page;
        public int App;
        public int Container;
        public Guid Block;
    }
}
