namespace ToSic.Sxc.Services.Tweaks
{
    public class TweakConfig<T> : TweakConfig
    {
        public T Tweak { get; }

        public TweakConfig(string nameId, T tweak, string step = default, string target = default) : base(nameId, step, target)
        {
            Tweak = tweak;
        }
    }
}
