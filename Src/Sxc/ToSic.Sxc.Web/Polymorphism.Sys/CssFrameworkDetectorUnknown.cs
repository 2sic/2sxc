﻿using Connect.Koi.Detectors;
using ToSic.Lib.Services;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Polymorphism.Sys;

internal class CssFrameworkDetectorUnknown(WarnUseOfUnknown<CssFrameworkDetectorUnknown> _) : ICssFrameworkDetector
{
    public string AutoDetect() => Connect.Koi.CssFrameworks.Unknown;
}