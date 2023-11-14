using ToSic.Lib.Documentation;
using static ToSic.Lib.Coding.StableApi;

namespace ToSic.Sxc.Services.Tweaks
{
    /// <summary>
    /// WIP 16.08 Helper to let a tweak operation modify a value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [PrivateApi("WIP 16.08")]
    public class TweakValue<TValue>: ITweakValue<TValue>
    {
        internal TweakValue(string name, string step, TValue initial)
        {
            Name = name;
            Step = step;
            //Initial = initial;
            Value = initial;
        }

        private TweakValue(TweakValue<TValue> original, TValue value)
        {
            Name = original.Name;
            Step = original.Step;
            //Initial = original.Initial;
            Value = value;
        }

        /// <summary>
        /// Name of the value which will be modified, eg `FirstName`
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Step of the tweak, like a workflow step. eg. `Result`
        /// </summary>
        public string Step { get; }

        ///// <summary>
        ///// Initial value before any processing had happened
        ///// </summary>
        //public TValue Initial { get; }

        /// <inheritdoc />
        public TValue Value { get; }

        /// <summary>
        /// Get a new TweakValue with the value changed.
        /// This is so that the object is always immutable.
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TweakValue<TValue> New(NoParamOrder noParamOrder = default, TValue value = default)
            => new TweakValue<TValue>(this, value);
    }
}
