using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.Polymorphism
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class PolymorphAttribute : Attribute
    {
        public string Name { get; }
        //public PolymorphType Type { get; set; }

        public PolymorphAttribute(string name)
        {
            this.Name = name;
        }
    }
}
