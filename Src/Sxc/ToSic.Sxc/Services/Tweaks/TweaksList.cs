using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Services.Tweaks
{
    internal class TweaksList
    {
        public TweaksList(TweakBase tweak)
        {
            List = new List<TweakBase> { tweak };
        }

        public TweaksList(TweaksList original, TweakBase additional = default) : this (original?.List, additional)
        {
        }

        private TweaksList(IEnumerable<TweakBase> tweaks, TweakBase additional = default)
        {
            List = tweaks?.ToList() ?? new();
            if (additional != default) List.Add(additional);
        }

        public List<TweakBase> List { get; }

        internal List<TweakBase> GetTweaksByName(string nameId)
            => List.Where(t => t.NameId == nameId).ToList();

        internal List<TweakBase> GetTweaksByStep(string step)
            => List.Where(t => t.Step == step).ToList();

    }
}
