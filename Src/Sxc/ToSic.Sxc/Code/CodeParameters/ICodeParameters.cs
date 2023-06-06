using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code
{
    public interface ICodeParameters
    {
        #region Get

        object Get(string name, string noParamOrder = Protector, bool required = false);

        T Get<T>(string name, string noParamOrder = Protector, T fallback = default, bool required = false);

        #endregion

        dynamic Dynamic(string name, string noParamOrder = Protector, object fallback = default, bool required = false);
        string String(string name, string noParamOrder = Protector, string fallback = default, bool required = false);

        #region Numbers

        int Int(string name, string noParamOrder = Protector, int fallback = default, bool required = false);
        float Float(string name, string noParamOrder = Protector, float fallback = default, bool required = false);
        double Double(string name, string noParamOrder = Protector, double fallback = default, bool required = false);
        decimal Decimal(string name, string noParamOrder = Protector, decimal fallback = default, bool required = false);
        #endregion

        Guid Guid(string name, string noParamOrder = Protector, Guid fallback = default, bool required = false);
        bool Bool(string name, string noParamOrder = Protector, bool fallback = default, bool required = false);
        DateTime DateTime(string name, string noParamOrder = Protector, DateTime fallback = default, bool required = false);
        ITypedFile File(string name, string noParamOrder = Protector, ITypedFile fallback = default, bool required = false);
        IEnumerable<ITypedFile> Files(string name, string noParamOrder = Protector, IEnumerable<ITypedFile> fallback = default, bool required = false);
        ITypedFolder Folder(string name, string noParamOrder = Protector, ITypedFolder fallback = default, bool required = false);
        IEnumerable<ITypedFolder> Folders(string name, string noParamOrder = Protector, IEnumerable<ITypedFolder> fallback = default, bool required = false);

        #region Item / Entity

        IEntity Entity(string name, string noParamOrder = Protector, IEntity fallback = default, bool required = false);

        ITypedItem Item(string name, string noParamOrder = Protector, ITypedItem fallback = default, bool required = false);
        IEnumerable<ITypedItem> Items(string name, string noParamOrder = Protector, IEnumerable<ITypedItem> fallback = default, bool required = false);

        #endregion

    }
}