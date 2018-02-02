// 2018-02-02 2dm - doesn't look used at all

//using System;
//using System.Globalization;
//using DotNetNuke.Entities.Modules;
//using DotNetNuke.Entities.Users;
//using DotNetNuke.Services.Tokens;
//using ToSic.Eav.ValueProvider;

//namespace ToSic.SexyContent.DataSources.Tokens
//{
//    public class ModulePropertyAccess : IValueProvider, IPropertyAccess
//    {
//        //private int _moduleId;
//        private readonly ModuleInfo _moduleInfo;

//	    public ModulePropertyAccess(string name)
//	    {
//		    Name = name;
//	    }

//	    public string Name { get; }

//        public ModulePropertyAccess(int moduleId)
//        {
//            //_moduleId = moduleId;
//            var ctr = new ModuleController();
//            _moduleInfo = ctr.GetModule(moduleId);
//        }

//        public string Get(string propertyName, string format, ref bool propertyNotFound)
//        {
//            return GetProperty(propertyName, "", null, null, Scope.DefaultSettings, ref propertyNotFound);
//        }
//        /// <summary>
//        /// Shorthand version, will return the string value or a null if not found. 
//        /// </summary>
//        /// <param name="property"></param>
//        /// <returns></returns>
//        public virtual string Get(string property)
//        {
//            var temp = false;
//            return Get(property, "", ref temp);
//        }


//        #region DotNetNuke IPropertyAccess Members

//        public CacheLevel Cacheability
//        {
//            get { return CacheLevel.notCacheable; }
//        }

//        public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo accessingUser, Scope accessLevel, ref bool propertyNotFound)
//        {
//            return _moduleInfo.GetProperty(propertyName, format, formatProvider, accessingUser, accessLevel, ref propertyNotFound);
//        }

//        #endregion

//        public bool Has(string property)
//        {
//            throw new NotImplementedException();
//        }

//    }
//}