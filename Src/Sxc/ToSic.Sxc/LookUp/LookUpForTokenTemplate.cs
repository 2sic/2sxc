using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.LookUp
{
    /// <summary>
    /// LookUp for creating token based templates. In addition to retrieving values, it also resolves special tokens like
    /// - repeater:index
    /// - repeater:isfirst
    /// - etc.
    /// </summary>
    /// <remarks>
    /// Only use this for Token templates, do not use for normal lookups which end up in data-sources.
    /// The reason is that this tries to respect culture formatting, which will cause trouble (numbers with comma etc.) when trying to
    /// use in other systems.
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public partial class LookUpForTokenTemplate : ILookUp
	{

        private readonly IDynamicEntity _entity;
		public string Name { get; }

	    private readonly int _repeaterIndex;
	    private readonly int _repeaterTotal;
        
        [PrivateApi]
		public LookUpForTokenTemplate(string name, IDynamicEntity entity, int repeaterIndex = -1, int repeaterTotal = -1)
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
        /// <returns></returns>
        private string GetProperty(string strPropertyName, string strFormat)
		{
			// Return empty string if Entity is null
			if (_entity == null)
				return string.Empty;

            var outputFormat = strFormat == string.Empty ? "g" : strFormat;

            // If it's a repeater token for list index or something, get that first (or null)
            // Otherwise just get the normal value
            var valueObject = ResolveRepeaterTokens(strPropertyName) 
                              ?? _entity.Get(strPropertyName);

            if (valueObject != null)
			{
				switch (Type.GetTypeCode(valueObject.GetType()))
				{
					case TypeCode.String:
						return LookUpBase.FormatString((string)valueObject, strFormat);
					case TypeCode.Boolean:
                        // #BreakingChange v11.11 - possible breaking change
                        // previously it converted true/false to language specific values
                        // but only in token templates (and previously also app-settings) which causes
                        // the use to have to be language aware - very complex
                        // I'm pretty sure this won't affect anybody
                        // old: ((bool)valueObject).ToString(formatProvider).ToLowerInvariant();
                        return LookUpBase.Format((bool) valueObject) ;
					case TypeCode.DateTime:
					case TypeCode.Double:
					case TypeCode.Single:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                        return ((IFormattable)valueObject).ToString(outputFormat, GetCultureInfo());
					default:
						return LookUpBase.FormatString(valueObject.ToString(), strFormat);
				}
			}

			#region Check for Navigation-Property (e.g. Manager:Name)

            if (!strPropertyName.Contains(':')) return string.Empty;

            var propertyMatch = Regex.Match(strPropertyName, "([a-z]+):([a-z]+)", RegexOptions.IgnoreCase);
            if (!propertyMatch.Success) return string.Empty;

            valueObject = _entity.Get(propertyMatch.Groups[1].Value);
            if (valueObject == null) return string.Empty;

            #region Handle Entity-Field (List of DynamicEntity)
            var list = valueObject as List<IDynamicEntity>;

            var entity = list?.FirstOrDefault() ?? valueObject as IDynamicEntity;

            if (entity != null)
                return new LookUpForTokenTemplate(null, entity).GetProperty(propertyMatch.Groups[2].Value, string.Empty);

            #endregion

            return string.Empty;
            #endregion

        }


        private CultureInfo GetCultureInfo() => IZoneCultureResolverExtensions.SafeCultureInfo(_entity?._Services.Dimensions);

        [PrivateApi]
		public string Get(string key, string strFormat) => GetProperty(key, strFormat);

        /// <inheritdoc/>
        public virtual string Get(string key) => Get(key, "");
    }
}