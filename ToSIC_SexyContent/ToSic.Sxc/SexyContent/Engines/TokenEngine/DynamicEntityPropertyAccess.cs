using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ToSic.Eav.ValueProvider;
using ToSic.Eav.ValueProviders;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.Engines.TokenEngine
{
	internal class DynamicEntityPropertyAccess : IValueProvider
	{
        public const string RepeaterSubToken = "Repeater";

		private readonly IDynamicEntity _entity;
		public string Name { get; private set; }

	    private readonly int _repeaterIndex = -1;
	    private readonly int _repeaterTotal = -1;
        

		public DynamicEntityPropertyAccess(string name, IDynamicEntity entity, int repeaterIndex = -1, int repeaterTotal = -1)
		{
			Name = name;
			_entity = entity;
		    _repeaterIndex = repeaterIndex;
		    _repeaterTotal = repeaterTotal;
		}

        /// <summary>
        /// Get Property out of NameValueCollection
        /// </summary>
        /// <param name="strPropertyName"></param>
        /// <param name="strFormat"></param>
        /// <param name="formatProvider"></param>
        /// <param name="propertyNotFound"></param>
        /// <returns></returns>
        public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider, ref bool propertyNotFound)
		{
			// Return empty string if Entity is null
			if (_entity == null)
				return string.Empty;

            var outputFormat = strFormat == string.Empty ? "g" : strFormat;

			var propNotFound = false;

            // 2015-05-04 2dm added functionality for repeater-infos
            string repeaterHelper = null;
            if (_repeaterIndex > -1 && strPropertyName.StartsWith(RepeaterSubToken + ":", StringComparison.OrdinalIgnoreCase))
            {
                switch (strPropertyName.Substring(RepeaterSubToken.Length + 1).ToLower())
                {
                    case "index":
                        repeaterHelper = (_repeaterIndex).ToString();
                        break;
                    case "index1":
                        repeaterHelper = (_repeaterIndex + 1).ToString();
                        break;
                    case "alternator2":
                        repeaterHelper = (_repeaterIndex%2).ToString();
                        break;
                    case "alternator3":
                        repeaterHelper = (_repeaterIndex%3).ToString();
                        break;
                    case "alternator4":
                        repeaterHelper = (_repeaterIndex%4).ToString();
                        break;
                    case "alternator5":
                        repeaterHelper = (_repeaterIndex%5).ToString();
                        break;
                    case "isfirst":
                        repeaterHelper = (_repeaterIndex == 0) ? "First" : "";
                        break;
                    case "islast":
                        repeaterHelper = (_repeaterIndex == _repeaterTotal - 1) ? "Last" : "";
                        break;
                    case "count":
                        repeaterHelper = _repeaterTotal.ToString();
                        break;
                }
            }

			var valueObject = repeaterHelper ?? _entity.Get(strPropertyName/*, out propertyNotFound*/);

			if (/*!propertyNotFound &&*/ valueObject != null)
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
					valueObject = _entity.Get(propertyMatch.Groups[1].Value/*, out propertyNotFound*/);
					if (/*!propertyNotFound &&*/ valueObject != null)
					{
						#region Handle Entity-Field (List of DynamicEntity)
						var list = valueObject as List<IDynamicEntity>;

                        var entity = list != null ? list.FirstOrDefault() : null;

                        if (entity == null)
                            entity = valueObject as IDynamicEntity;

						if (entity != null)
                            return new DynamicEntityPropertyAccess(null, entity).GetProperty(propertyMatch.Groups[2].Value, string.Empty, formatProvider, /*AccessingUser,*/ /*AccessLevel,*/ ref propNotFound);

						#endregion

                        return string.Empty;
                    }
				}
			}
			#endregion

			propertyNotFound = true;
			return string.Empty;
		}

		public string Get(string strPropertyName, string strFormat, ref bool propertyNotFound) 
            => GetProperty(strPropertyName, strFormat, Thread.CurrentThread.CurrentCulture, ref propertyNotFound);

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