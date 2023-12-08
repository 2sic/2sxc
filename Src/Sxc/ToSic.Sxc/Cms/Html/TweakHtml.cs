using System;
using System.Linq;
using ToSic.Lib.Coding;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Cms.Html
{
    internal class TweakHtml: ITweakHtml
    {
        public const string NameDefault = "default";
        public const string StepBefore = "before";
        public const string StepAfter = "after";

        public TweakHtml(TweakHtml original = default, TweakBase additional = default)
        {
            Tweaks = new TweaksList(original?.Tweaks, additional);
        }

        public TweaksList Tweaks { get; }

        public ITweakHtml Value(string replace, NoParamOrder protector = default, string step = default)
            => Value(_ => replace, step: step);

        public ITweakHtml Value(Func<string> func, NoParamOrder protector = default, string step = default)
            => Value(_ => func(), step: step);

        public ITweakHtml Value(Func<ITweakValue<string>, string> func, NoParamOrder protector = default, string step = default)
            => AddTweak(func, NameDefault, step ?? StepBefore, target: TweakBase.TargetDefault);




        private TweakHtml AddTweak(Func<ITweakValue<string>, string> changeFunc, string nameId, string step, string target)
        {
            var tweak = new TweakBase<Func<ITweakValue<string>, int, ITweakValue<string>>>(nameId,
                (v, index) => new TweakValue<string>(v, changeFunc(v), index), step, target);
            return new TweakHtml(this, tweak);
        }

        internal ITweakValue<string> Preprocess(string html, string name = NameDefault) 
            => Process(html, name, StepBefore);

        private ITweakValue<string> Process(string value, string name, string step)
        {
            var tweaks = Tweaks.GetTweaksByStep(step)
                .Select(t => t as TweakBase<Func<ITweakValue<string>, int, ITweakValue<string>>>)
                .Where(t => t != null)
                .Select((tweak, id) => new { tweak, id })
                .ToList();

            var start = new TweakValue<string>(value, name, step, 0) as ITweakValue<string>;
            return tweaks.Aggregate(start, (current, tweak) =>
            {
                try
                {
                    return tweak.tweak.Tweak(current, tweak.id);
                }
                catch (Exception e)
                {
                    var exMore = new Exception($"Error in tweak #{tweak.id} '{tweak.tweak.NameId}' at step '{tweak.tweak.Step}' for target '{tweak.tweak.Target}'", e);
                    throw exMore;
                }
            });
        }
    }
}
