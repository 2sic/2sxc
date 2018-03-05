using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
//using DotNetNuke.Entities.Users;
// using DotNetNuke.Services.Tokens;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.Engines.TokenEngine
{
	public class DynamicEntityPropertyAccess : /*IPropertyAccess,*/ IValueProvider
	{
        public const string RepeaterSubToken = "Repeater";

		private readonly DynamicEntity _entity;
		public string Name { get; private set; }

	    private int RepeaterIndex = -1;
	    private int RepeaterTotal = -1;
        

		public DynamicEntityPropertyAccess(string name, DynamicEntity entity, int repeaterIndex = -1, int repeaterTotal = -1)
		{
			Name = name;
			_entity = entity;
		    RepeaterIndex = repeaterIndex;
		    RepeaterTotal = repeaterTotal;
		}

		//public CacheLevel Cacheability
		//{
		//	get { return CacheLevel.notCacheable; }
		//}

		/// <summary>
		/// Get Property out of NameValueCollection
		/// </summary>
		/// <param name="strPropertyName"></param>
		/// <param name="strFormat"></param>
		/// <param name="formatProvider"></param>
		/// <param name="AccessingUser"></param>
		/// <param name="AccessLevel"></param>
		/// <param name="PropertyNotFound"></param>
		/// <returns></returns>
		public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider, /*UserInfo AccessingUser,*/ /*Scope AccessLevel,*/ ref bool PropertyNotFound)
		{
			// Return empty string if Entity is null
			if (_entity == null)
				return string.Empty;


            // 2015-04-25 - 2dm - must discuss w/2rm
            // Looks like very duplicate code! Try this instead
		    // return _entity.GetEntityValue(strPropertyName);

			var outputFormat = strFormat == string.Empty ? "g" : strFormat;

			bool propertyNotFound = false;

            // 2015-05-04 2dm added functionality for repeater-infos
            string repeaterHelper = null;
            if (RepeaterIndex > -1 && strPropertyName.StartsWith(RepeaterSubToken + ":", StringComparison.OrdinalIgnoreCase))
            {
                switch (strPropertyName.Substring(RepeaterSubToken.Length + 1).ToLower())
                {
                    case "index":
                        repeaterHelper = (RepeaterIndex).ToString();
                        break;
                    case "index1":
                        repeaterHelper = (RepeaterIndex + 1).ToString();
                        break;
                    case "alternator2":
                        repeaterHelper = (RepeaterIndex%2).ToString();
                        break;
                    case "alternator3":
                        repeaterHelper = (RepeaterIndex%3).ToString();
                        break;
                    case "alternator4":
                        repeaterHelper = (RepeaterIndex%4).ToString();
                        break;
                    case "alternator5":
                        repeaterHelper = (RepeaterIndex%5).ToString();
                        break;
                    case "isfirst":
                        repeaterHelper = (RepeaterIndex == 0) ? "First" : "";
                        break;
                    case "islast":
                        repeaterHelper = (RepeaterIndex == RepeaterTotal - 1) ? "Last" : "";
                        break;
                    case "count":
                        repeaterHelper = RepeaterTotal.ToString();
                        break;
                }
            }

			var valueObject = repeaterHelper ?? _entity.GetEntityValue(strPropertyName, out propertyNotFound);

			if (!propertyNotFound && valueObject != null)
			{
				switch (valueObject.GetType().Name)
				{
					case "String":
						return BaseValueProvider.FormatString((string)valueObject, strFormat);
					case "Boolean":
						return ((bool)valueObject).ToString(formatProvider).ToLower();
					case "DateTime":
					case "Double":
					case "Single":
					case "Int32":
					case "Int64":
					case "Decimal":
						return ((IFormattable)valueObject).ToString(outputFormat, formatProvider);
					default:
						return BaseValueProvider.FormatString(valueObject.ToString(), strFormat);
				}
			}

			#region Check for Navigation-Property (e.g. Manager:Name)
			if (strPropertyName.Contains(':'))
			{
				var propertyMatch = Regex.Match(strPropertyName, "([a-z]+):([a-z]+)", RegexOptions.IgnoreCase);
				if (propertyMatch.Success)
				{
					valueObject = _entity.GetEntityValue(propertyMatch.Groups[1].Value, out propertyNotFound);
					if (!propertyNotFound && valueObject != null)
					{
						#region Handle Entity-Field (List of DynamicEntity)
						var list = valueObject as List<DynamicEntity>;

                        var entity = list != null ? list.FirstOrDefault() : null;

                        if (entity == null)
                            entity = valueObject as DynamicEntity;

						if (entity != null)
                            return new DynamicEntityPropertyAccess(null, entity).GetProperty(propertyMatch.Groups[2].Value, string.Empty, formatProvider, /*AccessingUser,*/ /*AccessLevel,*/ ref propertyNotFound);

						#endregion

                        return string.Empty;
                    }
				}
			}
			#endregion

			PropertyNotFound = true;
			return string.Empty;
		}

		public string Get(string strPropertyName, string strFormat, ref bool PropertyNotFound)
		{
			return GetProperty(strPropertyName, strFormat, Thread.CurrentThread.CurrentCulture, /*null,*/ /*Scope.DefaultSettings,*/ ref PropertyNotFound);
		}

        /// <summary>
        /// Shorthand version, will return the string value or a null if not found. 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public virtual string Get(string property)
        {
            var temp = false;
            return Get(property, "", ref temp);
        }


        public bool Has(string property)
        {
            throw new NotImplementedException();
        }

	}
}