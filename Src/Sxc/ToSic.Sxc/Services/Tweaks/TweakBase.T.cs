namespace ToSic.Sxc.Services.Tweaks
{
    public class TweakBase<T> : TweakBase
    {
        public T Tweak { get; }

        public TweakBase(string nameId, T tweak, string step = default, string target = default) : base(nameId, step, target)
        {
            Tweak = tweak;
        }
    }
}
