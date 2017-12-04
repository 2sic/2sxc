namespace ToSic.SexyContent.Interfaces
{
    public interface IApp: Eav.Apps.Interfaces.IApp
    {
         //string Name { get; }
         //string Folder { get; }
         //bool Hidden { get; }
         dynamic Configuration { get;  }

        dynamic Settings { get;  }

        dynamic Resources { get;  }

         //string AppGuid { get; }

        string Path { get; }

        string PhysicalPath { get; }

        //IAppData Data { get; }

        //IDictionary<string, IDataSource> Query { get; }

        string Thumbnail { get; }
    }
}
