namespace ToSic.Sxc.Images;

public interface ITweakImage
{
    public ITweakImage LightboxEnable(bool isEnabled = true);

    public ITweakImage LightboxGroup(string group);

    public ITweakImage LightboxDescription(string description);

}