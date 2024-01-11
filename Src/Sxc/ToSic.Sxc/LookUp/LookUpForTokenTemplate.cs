using System.Globalization;
using System.Text.RegularExpressions;
using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.LookUp;

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
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[method: PrivateApi]
public partial class LookUpForTokenTemplate(
    string name,
    IDynamicEntity entity,
    int repeaterIndex = -1,
    int repeaterTotal = -1)
    : ILookUp
{
    public string Name { get; } = name;

    /// <summary>
    /// Get Property out of NameValueCollection
    /// </summary>
    /// <param name="strPropertyName"></param>
    /// <param name="strFormat"></param>
    /// <returns></returns>
    private string GetProperty(string strPropertyName, string strFormat)
    {
        // Return empty string if Entity is null
        if (entity == null)
            return string.Empty;

        var outputFormat = strFormat == string.Empty ? "g" : strFormat;

        // If it's a repeater token for list index or something, get that first (or null)
        // Otherwise just get the normal value
        var valueObject = ResolveRepeaterTokens(strPropertyName) 
                          ?? entity.Get(strPropertyName);

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

        valueObject = entity.Get(propertyMatch.Groups[1].Value);
        if (valueObject == null) return string.Empty;

        #region Handle Entity-Field (List of DynamicEntity)
        var list = valueObject as List<IDynamicEntity>;

        var entity1 = list?.FirstOrDefault() ?? valueObject as IDynamicEntity;

        if (entity1 != null)
            return new LookUpForTokenTemplate(null, entity1).GetProperty(propertyMatch.Groups[2].Value, string.Empty);

        #endregion

        return string.Empty;
        #endregion

    }


    private CultureInfo GetCultureInfo() => IZoneCultureResolverExtensions.SafeCultureInfo(entity?.Cdf.Dimensions);

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public string Get(string key, string strFormat) => GetProperty(key, strFormat);

    /// <inheritdoc/>
    public virtual string Get(string key) => Get(key, "");
}