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
