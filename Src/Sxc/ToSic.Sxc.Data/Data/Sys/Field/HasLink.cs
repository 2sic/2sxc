﻿namespace ToSic.Sxc.Data.Sys.Field;

/// <summary>
/// Special helper object pass around a url when it started as a string
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HasLink: IHasLink
{
    internal HasLink(string url)
        => Url = url;

    public string Url { get; }
}