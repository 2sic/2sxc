using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.SexyContent.Environment.Interfaces;

namespace ToSic.SexyContent.Environment.Base
{
    public class Culture: ICulture
    {
        public Culture(string code, string text, bool active, bool allowChange)
        {
            Code = code;
            Text = text;
            Active = active;
            AllowStateChange = allowChange;
        }

        public string Code { get;  }
        public string Text { get;  }
        public bool Active { get;  }
        public bool AllowStateChange { get;  }
        
    }
}