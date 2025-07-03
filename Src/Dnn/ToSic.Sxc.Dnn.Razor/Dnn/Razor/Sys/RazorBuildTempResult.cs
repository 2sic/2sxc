﻿namespace ToSic.Sxc.Dnn.Razor.Sys;

internal class RazorBuildTempResult<T>(T instance, bool usesHotBuild)
{
    public T Instance { get; set; } = instance;
    public bool UsesHotBuild { get; set; } = usesHotBuild;
}