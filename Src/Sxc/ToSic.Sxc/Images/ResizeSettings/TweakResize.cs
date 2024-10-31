namespace ToSic.Sxc.Images;

internal record TweakResize(ResizeSettings Settings): ITweakResize
{
    public ITweakResize Width(int width)
        => new TweakResize(Settings: Settings with { Width = width, UseFactorMap = false });

    public ITweakResize Height(int height)
        => new TweakResize(Settings: Settings with { Height = height, UseAspectRatio = false });

    public ITweakResize Factor(double factor)
        => new TweakResize(Settings: Settings with { Factor = factor });

    public ITweakResize Factor(string factor)
    {
        var f = ResizeParams.FactorOrNull(factor);
        return f == null ? this : new(Settings: Settings with { Factor = f.Value });
    }

    public ITweakResize AspectRatio(double aspectRatio)
        => new TweakResize(Settings: Settings with { AspectRatio = aspectRatio });

    public ITweakResize AspectRatio(string aspectRatio)
    {
        var a = ResizeParams.AspectRatioOrNull(aspectRatio);
        return a == null ? this : new(Settings: Settings with { AspectRatio = a.Value });
    }

    public ITweakResize Quality(double quality)
    {
        var q = ResizeParams.QualityOrNull(quality);
        return q == null ? this : new(Settings: Settings with { Quality = q.Value });
    }

    public ITweakResize ResizeMode(string resizeMode)
        => new TweakResize(Settings: Settings with { ResizeMode = resizeMode });

    public ITweakResize ScaleMode(string scaleMode)
    {
        var s = ResizeParams.ScaleModeOrNull(scaleMode);
        return s == null ? this : new(Settings: Settings with { ScaleMode = s });
    }

    public ITweakResize Format(string format)
    {
        var f = ResizeParams.FormatOrNull(format);
        return f == null ? this : new(Settings: Settings with { Format = f });
    }

    public ITweakResize Parameters(string parameters)
    {
        var p = ResizeParams.ParametersOrNull(parameters);
        return p == null ? this : new(Settings: Settings with { Parameters = p });
    }
}