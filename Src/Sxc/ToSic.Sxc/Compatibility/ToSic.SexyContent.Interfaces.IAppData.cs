#if NETFRAMEWORK
using System;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Interfaces
{
    [Obsolete("please use the Eav.Apps.Interfaces.IAppData instead")]
    public interface IAppData: Eav.Apps.IAppData { }
}

#endif