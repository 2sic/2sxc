﻿using System.Globalization;
using System.Text.RegularExpressions;
using ToSic.Eav.LookUp;
using ToSic.Eav.LookUp.Sources;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Engines.Sys.Token;

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
[ShowApiWhenReleased(ShowApiMode.Never)]
[method: PrivateApi]
internal partial class LookUpForTokenTemplate(
    string name,
    IDynamicEntity dynEntity,
    CultureInfo cultureInfo,
    int repeaterIndex = -1,
    int repeaterTotal = -1)
    : ILookUp
{
    public string Name { get; } = name ?? throw new NullReferenceException("LookUp must have a Name");

    public string Description => "LookUp for creating token based templates. In addition to retrieving values, it also resolves special tokens like repeater:index, repeater:isfirst, etc.";

    /// <summary>
    /// Get Property out of NameValueCollection
    /// </summary>
    /// <param name="key"></param>
    /// <param name="strFormat"></param>
    /// <returns></returns>
    private string? GetProperty(string key, string strFormat)
    {

        // As of 2025-05-13 v20 (2dm) the toolbar will only work in DNN
        // and not in Oqtane, since the DynamicEntity.Toolbar doesn't exist there
        // since we don't really plan to support token templates in future
        // this is acceptable for now.
#if NETFRAMEWORK
        // Create Toolbar if requested, even if dynEntity is null
        if (key == ViewConstants.FieldToolbar)
#pragma warning disable CS0618 // Type or member is obsolete
            return (dynEntity as ToSic.Sxc.Data.Sys.DynamicEntity)?.Toolbar.ToString();
#pragma warning restore CS0618 // Type or member is obsolete
        // old till 2025-05-13, changed to not depend on the ItemToolbar object
        // return new Edit.Toolbar.ItemToolbar(dynEntity?.Entity).ToolbarAsTag;

#endif
        // Return empty string if Entity is null
        if (dynEntity == null! || key.IsEmptyOrWs())
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
            string str => LookUpHelpers.FormatString(str, strFormat),
            bool b => LookUpHelpers.Format(b),
            DateTime or double or float or short or int or long or decimal =>
                ((IFormattable)valueObject).ToString(strFormat.NullIfNoValue() ?? "g", cultureInfo),
            _ => LookUpHelpers.FormatString(valueObject.ToString(), strFormat),
        };
    }

    private string TryGetSubProperty(string strPropertyName)
    {
        // dynamic valueObject;
        var propertyMatch = Regex.Match(strPropertyName, "([a-z]+):([a-z]+)", RegexOptions.IgnoreCase);
        if (!propertyMatch.Success)
            return "";

        var subSource = propertyMatch.Groups[1].Value;
        var subProp = propertyMatch.Groups[2].Value;

        // Find the sub-object on the Presentation item
        var subEntity = subSource.EqualsInsensitive(ViewParts.Presentation)
            ? dynEntity.Presentation as IDynamicEntity
            : dynEntity.Get(subSource) as IDynamicEntity;

        if (subEntity == null)
            return "";

        var subLookup = new LookUpForTokenTemplate("NoName", subEntity, cultureInfo);

        return subLookup.GetProperty(subProp, "") ?? "";
    }

    public string Get(string key, string strFormat)
        => GetProperty(key, strFormat) ?? "";

    /// <inheritdoc/>
    public virtual string Get(string key)
        => Get(key, "") ?? "";

    ILookUp ICanBeLookUp.LookUp => this;
}