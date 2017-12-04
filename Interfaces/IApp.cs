namespace ToSic.SexyContent.Interfaces
{
    public interface IApp: Eav.Apps.Interfaces.IApp
    {
         dynamic Configuration { get;  }

        dynamic Settings { get;  }

        dynamic Resources { get;  }

        string Path { get; }

        string PhysicalPath { get; }

        string Thumbnail { get; }
    }
}
