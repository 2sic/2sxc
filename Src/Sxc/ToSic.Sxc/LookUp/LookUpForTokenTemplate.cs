using System.Globalization;
using System.Text.RegularExpressions;
using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
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
internal partial class LookUpForTokenTemplate(
    string name,
    IDynamicEntity dynEntity,
    ICodeApiService codeApiService,
    int repeaterIndex = -1,
    int repeaterTotal = -1)
    : ILookUp
{
    private ILookUp _lookUp;
    public string Name { get; } = name;

    public string Description => "LookUp for creating token based templates. In addition to retrieving values, it also resolves special tokens like repeater:index, repeater:isfirst, etc.";

    /// <summary>
    /// Get Property out of NameValueCollection
    /// </summary>
    /// <param name="key"></param>
    /// <param name="strFormat"></param>
    /// <returns></returns>
    private string GetProperty(string key, string strFormat)
    {
        // Create Toolbar if requested, even if dynEntity is null
        if (key == ViewConstants.FieldToolbar)
            return new Edit.Toolbar.ItemToolbar(dynEntity?.Entity).ToolbarAsTag;

        // Return empty string if Entity is null
        if (dynEntity == null || key.IsEmptyOrWs())
            return "";

        // If we have a delimiter in the key, then we must check for sub-properties
        if (key.Contains(':'))
            return TryGetSubProperty(key);

        // If it's a repeater token for list index or something, get that first (or null)
        // Otherwise just get the normal value
        var valueObject = ResolveRepeaterTokens(key) 
                          ?? dynEntity.Get(key);

        return valueObject switch
        {
            null => string.Empty,
            string str => LookUpBase.FormatString(str, strFormat),
            bool b => LookUpBase.Format(b),
            DateTime or double or float or short or int or long or decimal =>
                ((IFormattable)valueObject).ToString(strFormat.NullIfNoValue() ?? "g", GetCultureInfo()),
            _ => (string)LookUpBase.FormatString(valueObject.ToString(), strFormat),
        };
    }

    private string TryGetSubProperty(string strPropertyName)
    {
        // dynamic valueObject;
        var propertyMatch = Regex.Match(strPropertyName, "([a-z]+):([a-z]+)", RegexOptions.IgnoreCase);
        if (!propertyMatch.Success) return "";

        var subSource = propertyMatch.Groups[1].Value;
        var subProp = propertyMatch.Groups[2].Value;

        // Find the sub-object on the Presentation item
        var subEntity = subSource.EqualsInsensitive(ViewParts.Presentation)
            ? dynEntity.Presentation as IDynamicEntity
            : dynEntity.Get(subSource) as IDynamicEntity;

        if (subEntity == null) return "";

        var subLookup = new LookUpForTokenTemplate(null, subEntity, codeApiService);

        return subLookup.GetProperty(subProp, "") ?? "";
    }


    private CultureInfo GetCultureInfo() => IZoneCultureResolverExtensions.SafeCultureInfo(codeApiService.Cdf.Dimensions);

    public string Get(string key, string strFormat) => GetProperty(key, strFormat);

    /// <inheritdoc/>
    public virtual string Get(string key) => Get(key, "");

    ILookUp ICanBeLookUp.LookUp => this;
}