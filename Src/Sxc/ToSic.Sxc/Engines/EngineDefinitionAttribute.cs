using System;

namespace ToSic.Sxc.Engines
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EngineDefinitionAttribute: Attribute
    {
        public EngineDefinitionAttribute()
        {

        }

        public string Name { get; set; } = Eav.Constants.NullNameId;


    }
}
