using System;
using System.Linq;
using System.Reflection;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.SharedCode
{
    /// <summary>
    /// inspired by
    /// https://stackoverflow.com/questions/2421994/invoking-methods-with-optional-parameters-through-reflection
    /// </summary>
    internal class CodeInvoker
    {
        public (bool Ok, object Result, int Count) Evaluate(object obj, string methodName, object[] args)
        {
            // Get the type of the object
            var t = obj.GetType();
            var argListTypes = args.Select(a => a.GetType()).ToArray();

            var allMethods = t.GetMethods();
            var funcs = allMethods
                .Where(m => m.Name.EqualsInsensitive(methodName))
                .Select(m => new { Method =  m, Specs = ArgumentListMatches(m, argListTypes) })
                .Where(x => x.Specs.Ok)
                .OrderBy(x => x.Specs.Score)
                .Select(x => x.Method)
                .ToArray();

            var count = funcs.Length;

            if (count == 0)
                return (false, null, count);

            //if (funcs.Length != 1)
            //    return (false, null, funcs.Length);

            // And invoke the method and see what we can get back.
            // Optional arguments means we have to fill things in.
            var method = funcs[0];
            var allArgs = args;
            if (method.GetParameters().Length != args.Length)
            {
                var defaultArgs = method.GetParameters().Skip(args.Length)
                    .Select(a => a.HasDefaultValue ? a.DefaultValue : null);
                allArgs = args.Concat(defaultArgs).ToArray();
            }
            var r = funcs[0].Invoke(obj, allArgs);
            return (true, r, funcs.Length);
        }

        public static (bool Ok, int Score) ArgumentListMatches(MethodInfo m, Type[] args)
        {
            // If there are less arguments, then it just doesn't matter.
            var pInfo = m.GetParameters();
            if (pInfo.Length < args.Length)
                return (false, 0);

            // Now, check compatibility of the first set of arguments.
            var commonArgs = args.Zip(pInfo, (type, paramInfo) => (ArgType: type, paramInfo.ParameterType));

            var weighed = commonArgs
                .Select(t => t.ParameterType.IsEquivalentTo(t.ArgType)
                    ? 100
                    // .ArgType.IsAssignableFrom(t.ParameterType)
                    : t.ParameterType.IsAssignableFrom(t.ArgType) 
                        ? 1
                        : 0)
                .ToList();

            //if (commonArgs.Where(t => !t.Item1.IsAssignableFrom(t.Item2)).Any())
            if (weighed.Any(w => w == 0))
                return (false, 0);

            // And make sure the last set of arguments are actually default!
            var missingAreOptional = pInfo
                .Skip(args.Length)
                .All(p => p.IsOptional);
            return (missingAreOptional, weighed.Sum());
        }
    }
}
