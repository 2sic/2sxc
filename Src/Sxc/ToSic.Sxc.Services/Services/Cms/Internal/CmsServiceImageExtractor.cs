﻿using System.Text.RegularExpressions;
using ToSic.Eav.Data.ValueConverter.Sys;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Web.Sys.HtmlParsing;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.CmsService.Internal;

internal class CmsServiceImageExtractor() : ServiceBase("Sxc.ImgExt")
{
    internal const string WysiwygLightboxClass = "wysiwyg-lightbox";

    internal ImagePropertiesExtracted ExtractImageProperties(string imgTag, IFolder folder)
    {
        var l = Log.Fn<ImagePropertiesExtracted>($"old: '{imgTag}'");
        string? src = null;
        string? factor = null;
        object? width = default;
        string? imgAlt = null;
        string? imgClasses = null;
        string? picClasses = null;
        var otherAttributes = new Dictionary<string, string?>();
        IFile? file = null;

        var files = folder.Files.ToList();

        // get all attributes
        var attributes = RegexUtil.AttributesDetection.Value.Matches(imgTag);
        foreach (Match attributeMatch in attributes)
        {
            var key = attributeMatch.Groups["Key"].Value;
            var value = attributeMatch.Groups["Value"].Value;
            switch (key.ToLowerInvariant())
            {
                case "data-cmsid":
                    var parts = new LinkParts(value, true);
                    src = parts.IsMatch ? $"{folder.Url}{parts.Name}" : value;
                    try
                    {
                        file = files.FirstOrDefault(f => f.FullName.EqualsInsensitive(parts.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Ex(ex, "Error while trying to get file from Adam");
                    }
                    break;
                case "src":
                    src ??= value; // should not overwrite data-cmsid
                    break;
                case "width":
                    width = value;
                    break;
                case "alt":
                    imgAlt = value;
                    break;
                case "class": // specially look at the classes
                    imgClasses = value; // add it as class
                    factor = GetImgServiceResizeFactor(value); // use the "#/#" as the `factor` parameter
                    picClasses = GetPictureClasses(value);
                    break;
                default:
                    // store alt-attribute, class etc. from the original if it had it (to re-attach latter)
                    otherAttributes[key] = value.NullIfNoValue();
                    break;
            }
        }

        var result = new ImagePropertiesExtracted
        {
            File = file,
            Src = src,
            Factor = factor,
            ImgAlt = imgAlt,
            ImgClasses = imgClasses,
            PicClasses = picClasses,
            Width = width,
            OtherAttributes = otherAttributes
        };
        return l.Return(result, $"src:{src}");
    }

    /// <summary>
    /// NOT DONE YET: the Picture tag should only preserve the wysiwyg-* classes
    /// </summary>
    /// <param name="classes"></param>
    /// <returns></returns>
    internal static string GetPictureClasses(string classes)
    {
        // TODO: filter to only return wysiwyg-* classes
        return classes;
    }

    /// <summary>
    /// ATM not used at all, only tested. Idea was that there would be a class to mark it for lightbox,
    /// but the decision was made to use image settings instead.
    /// </summary>
    /// <param name="classes"></param>
    /// <returns></returns>
    internal static bool UseLightbox(string? classes)
        => classes?.Contains(WysiwygLightboxClass) ?? false;

    internal static string? GetImgServiceResizeFactor(string value)
    {
        // check if we can find something like "wysiwyg-width#of#" - this is for resize ratios
        var widthMatch = RegexUtil.WysiwygWidthLazy.Value.Match(value);

        // convert to a format like "#/#"
        if (!widthMatch.Success)
            return null;

        var numString = widthMatch.Groups["percent"].Value;
        // We want to return a nice factor, in case the rules have optimized values
        return numString switch
        {
            "100" => "1",
            "50" => "1/2",
            "33" => "1/3",
            "66" => "2/3",
            "25" => "1/4",
            "75" => "3/4",
            _ => numString
        };
    }

    internal class ImagePropertiesExtracted
    {
        public IFile? File { get; init; }
        public string? Src { get; init; }
        public string? Factor { get; init; }
        public string? ImgAlt { get; init; }
        public string? ImgClasses { get; init; }
        public string? PicClasses { get; init; }
        public object? Width { get; init; }
        public required Dictionary<string, string?> OtherAttributes { get; init; }
    }
}