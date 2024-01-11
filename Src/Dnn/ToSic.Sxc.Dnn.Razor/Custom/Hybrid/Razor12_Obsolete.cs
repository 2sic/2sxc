using static ToSic.Sxc.Code.Internal.CodeErrorHelp.HelpForRazor12;

 // ReSharper disable once CheckNamespace
 namespace Custom.Hybrid; 

 // Important Note
 // The new hybrid implementation doesn't actually need this
 // But if we add these overloads in an inherited class, they will be preferred to the real working ones
 // which would result in errors on AsDynamic(some-object) even though it should just work
 partial class Razor12
 {
     // Obsolete stuff - not supported any more in RazorPage10 - maybe re-activate to show helpful error messages

     #region Shared Code Block between RazorComponent_Obsolete and ApiController_Obsolete

     #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10

     [PrivateApi]
     [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
     public dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
         => ExAsDynamicInterfacesIEntity();


     [PrivateApi]
     [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
     public dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
         => AsDynamicKvpInterfacesIEntity();

     [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
     [PrivateApi]
     public IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
         => AsDynamicIEnumInterfacesIEntity();

     #endregion

     #region AsDynamic<int, IEntity>

     [PrivateApi]
     [Obsolete("throws error with fix-instructions. Use AsDynamic(IEnumerable<IEntity>...)")]
     public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => ExAsDynamicKvp();

     #endregion


     #endregion

     //[PrivateApi("this is the old signature, should still be supported")]
     // we're not creating an error/overload here, because it may lead to signature issues 
     // where two methods have almost the same signature
     //public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate)
     //    => throw new Exception($"ListPresentation {NotSupportedIn10}. Use Header.Presentation");

     #region Old AsDynamic with correct warnings

     [PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataStream stream) => ExAsDynamicForList();
     [PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataSource source) => ExAsDynamicForList();
     [PrivateApi] public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => ExAsDynamicForList();

     #endregion
 }