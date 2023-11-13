using System;

namespace ToSic.Sxc.Engines
{
    [AttributeUsage(AttributeTargets.Class)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class EngineDefinitionAttribute: Attribute
    {
        public EngineDefinitionAttribute()
        {

        }

        public string Name { get; set; } = Eav.Constants.NullNameId;


    }
}
