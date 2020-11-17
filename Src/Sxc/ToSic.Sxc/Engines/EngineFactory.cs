using System;
using ToSic.Eav;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Engines
{
    internal class EngineFactory
    {
        public enum EngineType
        {
            Token = 0,
            Razor = 1
        }

        public static IEngine CreateEngine(IView view)
        {
            var engineType = view.IsRazor ? RazorEngine : typeof(TokenEngine);
            
            if (engineType == null)
                throw new Exception("Error: Could not find the template engine to parse this template.");

            return Factory.Resolve(engineType) as IEngine;
        }

        /// <summary>
        /// Look up the engine once in the whole application lifecycle, then re-use
        /// </summary>
        private static Type RazorEngine => _razorEngine ?? (_razorEngine = Factory.StaticBuild<IEngineFinder>().RazorEngineType());
        private static Type _razorEngine;

    }
}