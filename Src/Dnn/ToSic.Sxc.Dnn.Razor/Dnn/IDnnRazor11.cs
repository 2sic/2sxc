﻿namespace ToSic.Sxc.Dnn;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IDnnRazor11
{
    /// <summary>
    /// Code-Behind of this .cshtml file - located in a file with the same name but ending in .code.cshtml
    /// </summary>
    dynamic Code { get; }
}