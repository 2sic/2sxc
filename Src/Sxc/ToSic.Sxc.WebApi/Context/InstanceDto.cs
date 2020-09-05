using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.WebApi.Context
{
    public class InstanceDto
    {
        public int Id;
        public int ModuleId;
        public bool ShowOnAllPages;
        public string Title;
        public bool IsDeleted;
        public PageDto Page;
    }
}
