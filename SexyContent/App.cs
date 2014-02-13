using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent
{
    /// <summary>
    /// The app class is currently only used to provide the App:Path placeholder in a template
    /// </summary>
    public class App
    {
        public App()
        {
            Name = "Content";
        }

        public string Name { get; internal set; }
        public string Path { get; internal set; }
        public string PhysicalPath { get; internal set; }
        // public dynamic Settings { get; internal set; }
        // public dynamic Resources { get; internal set; }
    }
}