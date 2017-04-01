using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.SexyContent.Environment.Interfaces
{
    public interface ICulture
    {

         string Code { get;  }
         string Text { get;  }
         bool Active { get;  }
         bool AllowStateChange { get;  }
    }
}
