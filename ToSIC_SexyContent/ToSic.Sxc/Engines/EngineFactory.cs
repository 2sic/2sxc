using System;
using System.Reflection;
using ToSic.Sxc.Engines.Token;
using ToSic.Sxc.Views;

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
            Type engineType = null;

            switch (view.IsRazor)
            {
                case true:
                    // TODO
                    // This isn't done well, a setup like the DataSources which are loaded from DLL and
                    // instantiated would be the more correct (and probably faster) way to do this
                    var engineAssembly = Assembly.Load("ToSic.SexyContent.Razor");
                    engineType = engineAssembly.GetType("ToSic.Sxc.Engines.Razor.RazorEngine");
                    break;
                case false:
                    // Load Token Engine
                    engineType = typeof(TokenEngine);
                    break;
            }


            if (engineType == null)
                throw new Exception("Error: Could not find the template engine to parse this template.");

            return (IEngine)Activator.CreateInstance(engineType, null);
        }

    }
}