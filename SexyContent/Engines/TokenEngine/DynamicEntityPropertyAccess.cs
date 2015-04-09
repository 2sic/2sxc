using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.Engines.TokenEngine
{
	public class DynamicEntityPropertyAccess : IPropertyAccess, IValueProvider
	{
		private readonly DynamicEntity _entity;
		public string Name { get; private set; }

		public DynamicEntityPropertyAccess(string name, DynamicEntity entity)
		{
			Name = name;
			_entity = entity;
		}

		public CacheLevel Cacheability
		{
			get { return CacheLevel.notCacheable; }
		}

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
		public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
		{
			// Return empty string if Entity is null
			if (_entity == null)
				return string.Empty;

			var outputFormat = strFormat == string.Empty ? "g" : strFormat;

			bool propertyNotFound;
			var valueObject = _entity.GetEntityValue(strPropertyName, out propertyNotFound);

			if (!propertyNotFound && valueObject != null)
			{
				switch (valueObject.GetType().Name)
				{
					case "String":
						return PropertyAccess.FormatString((string)valueObject, strFormat);
					case "Boolean":
						return ((bool)valueObject).ToString(formatProvider).ToLower();
					case "DateTime":
					case "Double":
					case "Single":
					case "Int32":
					case "Int64":
					case "Decimal":
						return (((IFormattable)valueObject).ToString(outputFormat, formatProvider));
					default:
						return PropertyAccess.FormatString(valueObject.ToString(), strFormat);
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
						if (list != null)
						{
							if (!list.Any())
								return string.Empty;

							return new DynamicEntityPropertyAccess(null, list.First()).GetProperty(propertyMatch.Groups[2].Value, string.Empty, formatProvider, AccessingUser, AccessLevel, ref propertyNotFound);
						}
						#endregion
					}
				}
			}
			#endregion

			PropertyNotFound = true;
			return string.Empty;
		}

		public string Get(string strPropertyName, string strFormat, ref bool PropertyNotFound)
		{
			return GetProperty(strPropertyName, strFormat, Thread.CurrentThread.CurrentCulture, null, Scope.DefaultSettings, ref PropertyNotFound);
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