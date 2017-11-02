using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.SexyContent.Interfaces
{
    public interface IAppData: IDataSource, IDataTarget
    {
        void Create(string contentTypeName, Dictionary<string, object> values, string userName = null);

        void Update(int entityId, Dictionary<string, object> values, string userName = null);

        void Delete(int entityId, string userName = null);

        IMetadataProvider Metadata { get; }
    }
}
