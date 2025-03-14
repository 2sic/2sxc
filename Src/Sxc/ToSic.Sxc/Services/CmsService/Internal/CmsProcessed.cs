﻿namespace ToSic.Sxc.Services.CmsService.Internal;

internal class CmsProcessed(bool isProcessed, string contents, string classes)
{
    public bool IsProcessed { get; set; } = isProcessed;

    public string Contents { get; set; } = contents;
    public string Classes { get; set; } = classes;

    public string DefaultTag { get; set; } = "div";
}